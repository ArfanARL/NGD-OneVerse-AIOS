$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $PSScriptRoot
$ApiProject = Join-Path $Root "backend\NGD.OneVerse.Api\NGD.OneVerse.Api.csproj"
$InfrastructureProject = Join-Path $Root "backend\NGD.OneVerse.Infrastructure\NGD.OneVerse.Infrastructure.csproj"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " NGD OneVerse AIOS - BUILD" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

Write-Host "`n[1/3] Restoring..." -ForegroundColor Yellow
dotnet restore $ApiProject

Write-Host "`n[2/3] Building..." -ForegroundColor Yellow
dotnet build $ApiProject --no-restore

Write-Host "`n[3/3] Checking EF migrations..." -ForegroundColor Yellow
dotnet ef migrations list `
    --project $InfrastructureProject `
    --startup-project $ApiProject

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host " BUILD COMPLETED SUCCESSFULLY" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
