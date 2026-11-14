[CmdletBinding()]
param()
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
Set-Location $root

Write-Host "`n=== NGD OneVerse DOCTOR ===" -ForegroundColor Cyan
dotnet --version
if ($LASTEXITCODE -ne 0) { throw "dotnet is unavailable." }

$required = @(
  ".\backend\NGD.OneVerse.slnx",
  ".\backend\NGD.OneVerse.Api\NGD.OneVerse.Api.csproj",
  ".\backend\NGD.OneVerse.Infrastructure\NGD.OneVerse.Infrastructure.csproj"
)
foreach ($p in $required) {
  if (!(Test-Path $p)) { throw "Missing required project file: $p" }
  Write-Host "PASS $p" -ForegroundColor Green
}
Write-Host "Doctor passed." -ForegroundColor Green
