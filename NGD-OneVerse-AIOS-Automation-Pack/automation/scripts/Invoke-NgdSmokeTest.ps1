[CmdletBinding()]
param([string]$BaseUrl = "http://localhost:5187")
$ErrorActionPreference = "Stop"

function Test-Endpoint([string]$Name,[string]$Url) {
  try {
    $r = Invoke-WebRequest $Url -UseBasicParsing -TimeoutSec 8
    Write-Host "PASS $Name -> HTTP $($r.StatusCode)" -ForegroundColor Green
    return $true
  } catch {
    Write-Host "FAIL $Name -> $($_.Exception.Message)" -ForegroundColor Red
    return $false
  }
}

Write-Host "`n=== NGD AUTOMATED SMOKE TEST ===" -ForegroundColor Cyan
$ok = @()
$ok += Test-Endpoint "Root" "$BaseUrl/"
$ok += Test-Endpoint "Swagger" "$BaseUrl/swagger/index.html"
$ok += Test-Endpoint "Health" "$BaseUrl/health"
$ok += Test-Endpoint "Organizations" "$BaseUrl/api/organizations"
$ok += Test-Endpoint "Sites" "$BaseUrl/api/sites"
$ok += Test-Endpoint "Departments" "$BaseUrl/api/departments"
$ok += Test-Endpoint "Employees" "$BaseUrl/api/employees"

if ($ok -contains $false) { throw "Smoke test failed." }
Write-Host "Smoke test passed." -ForegroundColor Green
