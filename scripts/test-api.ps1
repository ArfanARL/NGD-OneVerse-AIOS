$ErrorActionPreference = "Stop"

$BaseUrl = "http://localhost:5187"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " NGD OneVerse AIOS - API TEST" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$tests = @(
    @{ Name = "Root";          Url = "$BaseUrl/" },
    @{ Name = "Health";        Url = "$BaseUrl/health" },
    @{ Name = "Swagger";       Url = "$BaseUrl/swagger/index.html" },
    @{ Name = "Organizations"; Url = "$BaseUrl/api/organizations" },
    @{ Name = "Sites";         Url = "$BaseUrl/api/sites" },
    @{ Name = "Departments";   Url = "$BaseUrl/api/departments" },
    @{ Name = "Employees";     Url = "$BaseUrl/api/employees" }
)

$passed = 0
$failed = 0

foreach ($test in $tests) {

    Write-Host "`n[$($test.Name)]" -ForegroundColor Yellow

    try {
        $response = Invoke-WebRequest `
            -Uri $test.Url `
            -UseBasicParsing `
            -TimeoutSec 10 `
            -ErrorAction Stop

        if ($response.StatusCode -ge 200 -and $response.StatusCode -lt 300) {
            Write-Host "PASS - HTTP $($response.StatusCode)" -ForegroundColor Green
            $passed++
        }
        else {
            Write-Host "FAIL - HTTP $($response.StatusCode)" -ForegroundColor Red
            $failed++
        }
    }
    catch {
        Write-Host "FAIL - $($_.Exception.Message)" -ForegroundColor Red
        $failed++
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " RESULTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

Write-Host "Passed : $passed" -ForegroundColor Green
Write-Host "Failed : $failed" -ForegroundColor Red

if ($failed -eq 0) {
    Write-Host ""
    Write-Host "ALL API TESTS PASSED" -ForegroundColor Green
    exit 0
}

Write-Host ""
Write-Host "API TESTS FAILED" -ForegroundColor Red
exit 1
