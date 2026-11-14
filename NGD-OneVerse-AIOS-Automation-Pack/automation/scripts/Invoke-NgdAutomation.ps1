[CmdletBinding()]
param(
  [ValidateSet("Validate","Restore","Build","Test","Migrate","Smoke","All")]
  [string]$Task="All",
  [switch]$StartApi
)
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
Set-Location $root
$solution = ".\backend\NGD.OneVerse.slnx"
$api = ".\backend\NGD.OneVerse.Api\NGD.OneVerse.Api.csproj"
$infra = ".\backend\NGD.OneVerse.Infrastructure\NGD.OneVerse.Infrastructure.csproj"

function Step([string]$Name,[scriptblock]$Action) {
  Write-Host "`n=== $Name ===" -ForegroundColor Cyan
  & $Action
  if ($LASTEXITCODE -ne 0) { throw "$Name failed." }
}

if ($Task -in @("Validate","All")) { Step "VALIDATE" { & ".\automation\scripts\Invoke-NgdDoctor.ps1" } }
if ($Task -in @("Restore","All"))  { Step "RESTORE" { dotnet restore $solution } }
if ($Task -in @("Build","All"))    { Step "BUILD" { dotnet build $solution --no-restore } }
if ($Task -in @("Test","All"))     { Step "TEST" { dotnet test $solution --no-restore } }
if ($Task -in @("Migrate","All"))  { Step "MIGRATE" { dotnet ef database update --project $infra --startup-project $api } }

if ($StartApi) {
  Write-Host "`n=== START API ===" -ForegroundColor Cyan
  dotnet run --project $api
  exit $LASTEXITCODE
}

if ($Task -in @("Smoke","All")) { Step "SMOKE" { & ".\automation\scripts\Invoke-NgdSmokeTest.ps1" } }
Write-Host "`n=== NGD AUTOMATION COMPLETE ===" -ForegroundColor Green
