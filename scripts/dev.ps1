$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$apiProject = Join-Path $root "backend\NGD.OneVerse.Api\NGD.OneVerse.Api.csproj"
$infraProject = Join-Path $root "backend\NGD.OneVerse.Infrastructure\NGD.OneVerse.Infrastructure.csproj"

Set-Location $root

Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host " NGD OneVerse - Automated Development Flow" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan

Write-Host ""
Write-Host "[1/6] Restoring solution..." -ForegroundColor Yellow
dotnet restore $apiProject

if ($LASTEXITCODE -ne 0) {
    throw "Restore failed."
}

Write-Host ""
Write-Host "[2/6] Building API..." -ForegroundColor Yellow
dotnet build $apiProject --no-restore

if ($LASTEXITCODE -ne 0) {
    throw "Build failed."
}

Write-Host ""
Write-Host "[3/6] Checking EF migrations..." -ForegroundColor Yellow
dotnet ef migrations list `
    --project $infraProject `
    --startup-project $apiProject

if ($LASTEXITCODE -ne 0) {
    throw "EF migration check failed."
}

Write-Host ""
Write-Host "[4/6] Applying database migrations..." -ForegroundColor Yellow
dotnet ef database update `
    --project $infraProject `
    --startup-project $apiProject

if ($LASTEXITCODE -ne 0) {
    throw "Database migration failed."
}

Write-Host ""
Write-Host "[5/6] Starting API..." -ForegroundColor Yellow
Write-Host ""
Write-Host "Swagger: http://localhost:5187/swagger" -ForegroundColor Green
Write-Host "API:     http://localhost:5187" -ForegroundColor Green
Write-Host ""
Write-Host "Press Ctrl+C to stop the API." -ForegroundColor DarkYellow
Write-Host ""

dotnet run --project $apiProject --no-build