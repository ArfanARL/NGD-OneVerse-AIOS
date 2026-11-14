$ErrorActionPreference = "Stop"

$Root = $PSScriptRoot

$Solution = Join-Path $Root "backend\NGD.OneVerse.slnx"
$ApiProject = Join-Path $Root "backend\NGD.OneVerse.Api\NGD.OneVerse.Api.csproj"
$InfrastructureProject = Join-Path $Root "backend\NGD.OneVerse.Infrastructure\NGD.OneVerse.Infrastructure.csproj"

$ApiUrl = "http://localhost:5050"

Write-Host ""
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "       NGD OneVerse - DEVELOPMENT AUTOMATION" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "[1/7] RESTORE" -ForegroundColor Yellow
dotnet restore $Solution

Write-Host ""
Write-Host "[2/7] BUILD" -ForegroundColor Yellow
dotnet build $Solution --no-restore

Write-Host ""
Write-Host "[3/7] EF MIGRATIONS" -ForegroundColor Yellow
dotnet ef migrations list `
    --project $InfrastructureProject `
    --startup-project $ApiProject

Write-Host ""
Write-Host "[4/7] DATABASE UPDATE" -ForegroundColor Yellow
dotnet ef database update `
    --project $InfrastructureProject `
    --startup-project $ApiProject

Write-Host ""
Write-Host "[5/7] STARTING API" -ForegroundColor Yellow

$ApiProcess = Start-Process `
    -FilePath "dotnet" `
    -ArgumentList "run --project `"$ApiProject`" --no-build --urls $ApiUrl" `
    -PassThru `
    -WindowStyle Normal

Write-Host "API process started. PID: $($ApiProcess.Id)" -ForegroundColor Green

Write-Host ""
Write-Host "[6/7] WAITING FOR API" -ForegroundColor Yellow

$Ready = $false

for ($i = 1; $i -le 30; $i++) {

    Start-Sleep -Seconds 1

    try {
        $response = Invoke-WebRequest `
            -Uri "$ApiUrl/swagger/index.html" `
            -UseBasicParsing `
            -TimeoutSec 2

        if ($response.StatusCode -eq 200) {
            $Ready = $true
            break
        }
    }
    catch {
        Write-Host "." -NoNewline
    }
}

Write-Host ""

if (-not $Ready) {
    Write-Host ""
    Write-Host "API did not become ready." -ForegroundColor Red
    Write-Host "Check the API process window for the error." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "[7/7] SYSTEM READY" -ForegroundColor Green
Write-Host ""
Write-Host "==================================================" -ForegroundColor Green
Write-Host " API       : $ApiUrl" -ForegroundColor Green
Write-Host " Swagger   : $ApiUrl/swagger" -ForegroundColor Green
Write-Host " PID       : $($ApiProcess.Id)" -ForegroundColor Green
Write-Host " DATABASE  : Updated" -ForegroundColor Green
Write-Host " BUILD     : Successful" -ForegroundColor Green
Write-Host "==================================================" -ForegroundColor Green
Write-Host ""
