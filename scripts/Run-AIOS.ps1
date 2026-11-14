$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$apiProject = Join-Path $root "backend\NGD.OneVerse.Api\NGD.OneVerse.Api.csproj"
$infraProject = Join-Path $root "backend\NGD.OneVerse.Infrastructure\NGD.OneVerse.Infrastructure.csproj"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " NGD OneVerse AIOS - AUTOMATION" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

Set-Location $root

Write-Host "`n[1/6] Building API..." -ForegroundColor Yellow
dotnet build $apiProject

if ($LASTEXITCODE -ne 0) {
    throw "BUILD FAILED"
}

Write-Host "`n[2/6] Checking EF migrations..." -ForegroundColor Yellow
dotnet ef migrations list `
    --project $infraProject `
    --startup-project $apiProject

if ($LASTEXITCODE -ne 0) {
    throw "MIGRATION CHECK FAILED"
}

Write-Host "`n[3/6] Applying database migrations..." -ForegroundColor Yellow
dotnet ef database update `
    --project $infraProject `
    --startup-project $apiProject

if ($LASTEXITCODE -ne 0) {
    throw "DATABASE UPDATE FAILED"
}

Write-Host "`n[4/6] Starting API..." -ForegroundColor Yellow

$apiProcess = Start-Process `
    dotnet `
    -ArgumentList "run --project `"$apiProject`" --no-launch-profile" `
    -PassThru `
    -WindowStyle Hidden

try {
    Write-Host "Waiting for API..." -ForegroundColor Gray

    $ready = $false

    for ($i = 1; $i -le 30; $i++) {
        Start-Sleep -Seconds 1

        try {
            $response = Invoke-WebRequest `
                "http://localhost:5187/health" `
                -UseBasicParsing `
                -TimeoutSec 2

            if ($response.StatusCode -eq 200) {
                $ready = $true
                break
            }
        }
        catch {
            # API is still starting
        }
    }

    if (-not $ready) {
        throw "API did not become ready within 30 seconds."
    }

    Write-Host "API READY" -ForegroundColor Green

    Write-Host "`n[5/6] Running smoke tests..." -ForegroundColor Yellow

    $tests = @(
        @{ Name = "Root";          Url = "http://localhost:5187/" },
        @{ Name = "Swagger";       Url = "http://localhost:5187/swagger/index.html" },
        @{ Name = "Health";        Url = "http://localhost:5187/health" },
        @{ Name = "Organizations"; Url = "http://localhost:5187/api/organizations" },
        @{ Name = "Sites";         Url = "http://localhost:5187/api/sites" },
        @{ Name = "Departments";   Url = "http://localhost:5187/api/departments" },
        @{ Name = "Employees";     Url = "http://localhost:5187/api/employees" }
    )

    $failed = $false

    foreach ($test in $tests) {
        try {
            $r = Invoke-WebRequest `
                $test.Url `
                -UseBasicParsing `
                -TimeoutSec 5

            Write-Host "PASS  $($test.Name) [$($r.StatusCode)]" -ForegroundColor Green
        }
        catch {
            Write-Host "FAIL  $($test.Name)" -ForegroundColor Red
            $failed = $true
        }
    }

    Write-Host "`n[6/6] Final result..." -ForegroundColor Yellow

    if ($failed) {
        throw "One or more smoke tests failed."
    }

    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host " NGD OneVerse AIOS - ALL TESTS PASSED" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "API:    http://localhost:5187" -ForegroundColor Cyan
    Write-Host "Health: http://localhost:5187/health" -ForegroundColor Cyan
    Write-Host "Swagger: http://localhost:5187/swagger" -ForegroundColor Cyan
    Write-Host ""

}
finally {
    if ($apiProcess -and !$apiProcess.HasExited) {
        Write-Host "Stopping API..." -ForegroundColor Gray
        Stop-Process -Id $apiProcess.Id -Force
    }
}
