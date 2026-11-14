[CmdletBinding()]
param(
    [ValidateSet("Validate","Restore","Build","Test","Migrate","Smoke","All")]
    [string]$Task = "All",

    [switch]$StartApi
)

$ErrorActionPreference = "Stop"

# ============================================================
# NGD OneVerse AIOS - Central Automation Runner
# ============================================================

$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
Set-Location $root

$solution = ".\backend\NGD.OneVerse.slnx"
$api      = ".\backend\NGD.OneVerse.Api\NGD.OneVerse.Api.csproj"
$infra    = ".\backend\NGD.OneVerse.Infrastructure\NGD.OneVerse.Infrastructure.csproj"

$apiUrl   = "http://localhost:5187"

$apiOutLog = ".\automation\api-out.log"
$apiErrLog = ".\automation\api-error.log"


# ============================================================
# Helper: Execute a pipeline step
# ============================================================

function Step(
    [string]$Name,
    [scriptblock]$Action
) {
    Write-Host "`n=== $Name ===" -ForegroundColor Cyan

    & $Action

    if ($LASTEXITCODE -ne 0) {
        throw "$Name failed."
    }
}


# ============================================================
# Helper: Stop existing NGD API processes
# ============================================================

function Stop-NgdApiProcesses {

    Write-Host "Checking for existing NGD API process..." -ForegroundColor DarkGray

    $processes = Get-CimInstance Win32_Process -ErrorAction SilentlyContinue |
        Where-Object {
            $_.Name -eq "dotnet.exe" -and
            $_.CommandLine -like "*NGD.OneVerse.Api.csproj*"
        }

    foreach ($process in $processes) {

        Write-Host `
            "Stopping existing API process PID $($process.ProcessId)" `
            -ForegroundColor Yellow

        Stop-Process `
            -Id $process.ProcessId `
            -Force `
            -ErrorAction SilentlyContinue
    }

    Start-Sleep -Seconds 1
}


# ============================================================
# VALIDATE
# ============================================================

if ($Task -in @("Validate", "All")) {

    Step "VALIDATE" {
        & ".\automation\scripts\Invoke-NgdDoctor.ps1"
    }
}


# ============================================================
# RESTORE
# ============================================================

if ($Task -in @("Restore", "All")) {

    Step "RESTORE" {
        dotnet restore $solution
    }
}


# ============================================================
# BUILD
# ============================================================

if ($Task -in @("Build", "All")) {

    Step "BUILD" {
        dotnet build $solution --no-restore
    }
}


# ============================================================
# TEST
# ============================================================

if ($Task -in @("Test", "All")) {

    Step "TEST" {
        dotnet test $solution --no-restore
    }
}


# ============================================================
# DATABASE MIGRATION
# ============================================================

if ($Task -in @("Migrate", "All")) {

    Step "MIGRATE" {

        dotnet ef database update `
            --project $infra `
            --startup-project $api
    }
}


# ============================================================
# START API
#
# All / Smoke automatically starts the API.
#
# Development environment is forced because:
# - Swagger is enabled in Development
# - launchSettings previously used port 5187
# - Development configuration contains the expected DB settings
# ============================================================

if ($StartApi -or $Task -in @("Smoke", "All")) {

    Write-Host "`n=== START API ===" -ForegroundColor Cyan

    # --------------------------------------------------------
    # Stop old API instances
    # --------------------------------------------------------

    Stop-NgdApiProcesses


    # --------------------------------------------------------
    # Remove previous logs
    # --------------------------------------------------------

    Remove-Item `
        $apiOutLog,
        $apiErrLog `
        -ErrorAction SilentlyContinue


    # --------------------------------------------------------
    # Force correct runtime configuration
    # --------------------------------------------------------

    $env:ASPNETCORE_ENVIRONMENT = "Development"
    $env:ASPNETCORE_URLS = $apiUrl


    # --------------------------------------------------------
    # Start API in background
    # --------------------------------------------------------

    Write-Host "Starting API in background..." -ForegroundColor Yellow
    Write-Host "URL: $apiUrl" -ForegroundColor DarkGray

    $apiProcess = Start-Process `
        -FilePath "dotnet" `
        -ArgumentList `
            "run --project `"$api`" --no-build --no-launch-profile" `
        -RedirectStandardOutput $apiOutLog `
        -RedirectStandardError $apiErrLog `
        -WorkingDirectory $root `
        -PassThru


    Write-Host "API PID: $($apiProcess.Id)" -ForegroundColor DarkGray


    # --------------------------------------------------------
    # Wait for health endpoint
    # --------------------------------------------------------

    Write-Host "Waiting for API health endpoint..." -ForegroundColor Yellow

    $healthy = $false

    for ($i = 1; $i -le 30; $i++) {

        Start-Sleep -Seconds 1

        try {

            $health = Invoke-WebRequest `
                "$apiUrl/health" `
                -UseBasicParsing `
                -TimeoutSec 2 `
                -ErrorAction Stop

            if ($health.StatusCode -eq 200) {

                $healthy = $true

                Write-Host `
                    "API healthy -> HTTP 200" `
                    -ForegroundColor Green

                break
            }
        }
        catch {
            # API is still starting.
        }
    }


    # --------------------------------------------------------
    # API failed to start
    # --------------------------------------------------------

    if (-not $healthy) {

        Write-Host "`n=== API STARTUP OUTPUT ===" -ForegroundColor Red

        if (Test-Path $apiOutLog) {
            Get-Content $apiOutLog
        }

        Write-Host "`n=== API STARTUP ERRORS ===" -ForegroundColor Red

        if (Test-Path $apiErrLog) {
            Get-Content $apiErrLog
        }


        if (Get-Process `
            -Id $apiProcess.Id `
            -ErrorAction SilentlyContinue) {

            Stop-Process `
                -Id $apiProcess.Id `
                -Force `
                -ErrorAction SilentlyContinue
        }

        throw "API did not become healthy within 30 seconds."
    }


    # ========================================================
    # SMOKE TEST
    # ========================================================

    if ($Task -in @("Smoke", "All") -or $StartApi) {

        Step "SMOKE" {

            & ".\automation\scripts\Invoke-NgdSmokeTest.ps1" `
                -BaseUrl $apiUrl
        }
    }


    # ========================================================
    # STOP API
    # ========================================================

    Write-Host "`nStopping API PID $($apiProcess.Id)..." -ForegroundColor Yellow

    Stop-Process `
        -Id $apiProcess.Id `
        -Force `
        -ErrorAction SilentlyContinue

    Start-Sleep -Seconds 1

    Write-Host "API stopped." -ForegroundColor Green


    # ========================================================
    # FINAL RESULT
    # ========================================================

    Write-Host "`n=== NGD AUTOMATION COMPLETE ===" -ForegroundColor Green

    exit 0
}


# ============================================================
# Tasks that don't require API startup
# ============================================================

if ($Task -eq "Smoke") {

    Step "SMOKE" {

        & ".\automation\scripts\Invoke-NgdSmokeTest.ps1" `
            -BaseUrl $apiUrl
    }
}


Write-Host "`n=== NGD AUTOMATION COMPLETE ===" -ForegroundColor Green