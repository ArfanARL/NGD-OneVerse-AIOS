$ErrorActionPreference = "Stop"

$Root = $PSScriptRoot

$ApiProject = Join-Path $Root "backend\NGD.OneVerse.Api\NGD.OneVerse.Api.csproj"
$InfrastructureProject = Join-Path $Root "backend\NGD.OneVerse.Infrastructure\NGD.OneVerse.Infrastructure.csproj"

Write-Host ""
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host " NGD OneVerse - Automated Development Build" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "[1/6] Restoring..." -ForegroundColor Yellow
dotnet restore $ApiProject

Write-Host ""
Write-Host "[2/6] Building API..." -ForegroundColor Yellow
dotnet build $ApiProject --no-restore

Write-Host ""
Write-Host "[3/6] Checking EF Core migrations..." -ForegroundColor Yellow
dotnet ef migrations list `
    --project $InfrastructureProject `
    --startup-project $ApiProject

Write-Host ""
Write-Host "[4/6] Updating database..." -ForegroundColor Yellow
dotnet ef database update `
    --project $InfrastructureProject `
    --startup-project $ApiProject

Write-Host ""
Write-Host "[5/6] Final build..." -ForegroundColor Yellow
dotnet build $ApiProject --no-restore

Write-Host ""
Write-Host "[6/6] SUCCESS" -ForegroundColor Green
Write-Host ""
Write-Host "NGD OneVerse backend is ready." -ForegroundColor Green
Write-Host ""
