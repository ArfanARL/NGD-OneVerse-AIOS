[CmdletBinding()]
param([ValidateSet("List","Apply","RemoveLast")][string]$Action="List")
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
Set-Location $root
$infra = ".\backend\NGD.OneVerse.Infrastructure\NGD.OneVerse.Infrastructure.csproj"
$api = ".\backend\NGD.OneVerse.Api\NGD.OneVerse.Api.csproj"

switch ($Action) {
  "List" { dotnet ef migrations list --project $infra --startup-project $api }
  "Apply" { dotnet ef database update --project $infra --startup-project $api }
  "RemoveLast" { dotnet ef migrations remove --project $infra --startup-project $api }
}
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
