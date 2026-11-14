$ErrorActionPreference = "Continue"

$root = (Get-Location).Path
$backend = Join-Path $root "backend"

Write-Host ""
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host " NGD ONEVERSE AIOS - BACKEND ARCHITECTURE AUDIT" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""

# ------------------------------------------------------------
# PROJECTS
# ------------------------------------------------------------
Write-Host "[1] PROJECTS" -ForegroundColor Yellow

Get-ChildItem $backend -Filter *.csproj -Recurse |
    ForEach-Object {
        Write-Host "  [OK] $($_.FullName.Replace($root + '\',''))" -ForegroundColor Green
    }

# ------------------------------------------------------------
# SOLUTION
# ------------------------------------------------------------
Write-Host ""
Write-Host "[2] SOLUTION" -ForegroundColor Yellow

$sln = Join-Path $backend "NGD.OneVerse.slnx"

if (Test-Path $sln) {
    Write-Host "  [OK] NGD.OneVerse.slnx" -ForegroundColor Green
}
else {
    Write-Host "  [MISSING] NGD.OneVerse.slnx" -ForegroundColor Red
}

# ------------------------------------------------------------
# ENTITIES
# ------------------------------------------------------------
Write-Host ""
Write-Host "[3] DOMAIN ENTITIES" -ForegroundColor Yellow

$entityDir = Join-Path $backend "NGD.OneVerse.Domain\Entities"

$expectedEntities = @(
    "User.cs",
    "Role.cs",
    "Permission.cs",
    "UserRole.cs",
    "RolePermission.cs",
    "Organization.cs",
    "Site.cs",
    "Department.cs",
    "Employee.cs"
)

foreach ($name in $expectedEntities) {
    $path = Join-Path $entityDir $name

    if (Test-Path $path) {
        Write-Host "  [OK] $name" -ForegroundColor Green
    }
    else {
        Write-Host "  [MISSING] $name" -ForegroundColor Red
    }
}

# ------------------------------------------------------------
# EF CONFIGURATIONS
# ------------------------------------------------------------
Write-Host ""
Write-Host "[4] EF CORE CONFIGURATIONS" -ForegroundColor Yellow

$configDir = Join-Path $backend "NGD.OneVerse.Infrastructure\Persistence\Configurations"

if (Test-Path $configDir) {
    Get-ChildItem $configDir -Filter *.cs |
        Sort-Object Name |
        ForEach-Object {
            Write-Host "  [OK] $($_.Name)" -ForegroundColor Green
        }
}
else {
    Write-Host "  [MISSING] Configurations directory" -ForegroundColor Red
}

# ------------------------------------------------------------
# CONTROLLERS
# ------------------------------------------------------------
Write-Host ""
Write-Host "[5] API CONTROLLERS" -ForegroundColor Yellow

$controllerDir = Join-Path $backend "NGD.OneVerse.Api\Controllers"

if (Test-Path $controllerDir) {
    Get-ChildItem $controllerDir -Filter *.cs |
        Sort-Object Name |
        ForEach-Object {
            Write-Host "  [OK] $($_.Name)" -ForegroundColor Green
        }
}
else {
    Write-Host "  [MISSING] Controllers directory" -ForegroundColor Red
}

# ------------------------------------------------------------
# SERVICES
# ------------------------------------------------------------
Write-Host ""
Write-Host "[6] API SERVICES" -ForegroundColor Yellow

$serviceDir = Join-Path $backend "NGD.OneVerse.Api\Services"

if (Test-Path $serviceDir) {
    Get-ChildItem $serviceDir -Filter *.cs |
        Sort-Object Name |
        ForEach-Object {
            Write-Host "  [OK] $($_.Name)" -ForegroundColor Green
        }
}
else {
    Write-Host "  [MISSING] Services directory" -ForegroundColor Red
}

# ------------------------------------------------------------
# AUTHORIZATION
# ------------------------------------------------------------
Write-Host ""
Write-Host "[7] AUTHORIZATION" -ForegroundColor Yellow

$authDir = Join-Path $backend "NGD.OneVerse.Api\Authorization"

if (Test-Path $authDir) {
    Get-ChildItem $authDir -Filter *.cs |
        Sort-Object Name |
        ForEach-Object {
            Write-Host "  [OK] $($_.Name)" -ForegroundColor Green
        }
}
else {
    Write-Host "  [MISSING] Authorization directory" -ForegroundColor Red
}

# ------------------------------------------------------------
# PERSISTENCE
# ------------------------------------------------------------
Write-Host ""
Write-Host "[8] PERSISTENCE" -ForegroundColor Yellow

$persistenceDir = Join-Path $backend "NGD.OneVerse.Infrastructure\Persistence"

$expectedPersistence = @(
    "ApplicationDbContext.cs",
    "DbSeeder.cs"
)

foreach ($name in $expectedPersistence) {
    $path = Join-Path $persistenceDir $name

    if (Test-Path $path) {
        Write-Host "  [OK] $name" -ForegroundColor Green
    }
    else {
        Write-Host "  [MISSING] $name" -ForegroundColor Red
    }
}

# ------------------------------------------------------------
# MIGRATIONS
# ------------------------------------------------------------
Write-Host ""
Write-Host "[9] EF MIGRATIONS" -ForegroundColor Yellow

$migrationsDir = Join-Path $backend "NGD.OneVerse.Infrastructure\Migrations"

if (Test-Path $migrationsDir) {
    $migrations = Get-ChildItem $migrationsDir -Filter *.cs

    if ($migrations.Count -gt 0) {
        foreach ($migration in $migrations) {
            Write-Host "  [OK] $($migration.Name)" -ForegroundColor Green
        }
    }
    else {
        Write-Host "  [WARNING] Migrations directory is empty" -ForegroundColor Yellow
    }
}
else {
    Write-Host "  [INFO] No Migrations directory found" -ForegroundColor Yellow
}

# ------------------------------------------------------------
# PROGRAM
# ------------------------------------------------------------
Write-Host ""
Write-Host "[10] PROGRAM / STARTUP" -ForegroundColor Yellow

$program = Join-Path $backend "NGD.OneVerse.Api\Program.cs"

if (Test-Path $program) {
    Write-Host "  [OK] Program.cs" -ForegroundColor Green

    $programText = Get-Content $program -Raw

    $checks = @(
        @{ Name = "Controllers"; Pattern = "AddControllers" },
        @{ Name = "DbContext"; Pattern = "AddDbContext" },
        @{ Name = "Authentication"; Pattern = "AddAuthentication" },
        @{ Name = "Authorization"; Pattern = "AddAuthorization" },
        @{ Name = "JWT Bearer"; Pattern = "AddJwtBearer" },
        @{ Name = "MapControllers"; Pattern = "MapControllers" },
        @{ Name = "Health"; Pattern = "MapHealthChecks" }
    )

    foreach ($check in $checks) {
        if ($programText -match $check.Pattern) {
            Write-Host "  [OK] $($check.Name)" -ForegroundColor Green
        }
        else {
            Write-Host "  [MISSING] $($check.Name)" -ForegroundColor Red
        }
    }
}
else {
    Write-Host "  [MISSING] Program.cs" -ForegroundColor Red
}

# ------------------------------------------------------------
# CONFIGURATION FILES
# ------------------------------------------------------------
Write-Host ""
Write-Host "[11] CONFIGURATION FILES" -ForegroundColor Yellow

$configPatterns = @(
    "appsettings*.json"
)

foreach ($pattern in $configPatterns) {
    Get-ChildItem $backend -Filter $pattern -Recurse |
        Sort-Object FullName |
        ForEach-Object {
            Write-Host "  [OK] $($_.FullName.Replace($root + '\',''))" -ForegroundColor Green
        }
}

# ------------------------------------------------------------
# PERMISSION ATTRIBUTE USAGE
# ------------------------------------------------------------
Write-Host ""
Write-Host "[12] PERMISSION AUTHORIZATION USAGE" -ForegroundColor Yellow

$apiDir = Join-Path $backend "NGD.OneVerse.Api"

$permissionMatches = Get-ChildItem $apiDir -Filter *.cs -Recurse |
    Select-String -Pattern "\[Permission\(" -ErrorAction SilentlyContinue

if ($permissionMatches) {
    foreach ($match in $permissionMatches) {
        Write-Host "  [FOUND] $($match.Path.Replace($root + '\','')):$($match.LineNumber)" -ForegroundColor Green
    }
}
else {
    Write-Host "  [INFO] No [Permission(...)] usage found" -ForegroundColor Yellow
}

# ------------------------------------------------------------
# BUILD
# ------------------------------------------------------------
Write-Host ""
Write-Host "[13] BUILD" -ForegroundColor Yellow

Push-Location $root

dotnet build ".\backend\NGD.OneVerse.slnx"

$buildExit = $LASTEXITCODE

Pop-Location

Write-Host ""

if ($buildExit -eq 0) {
    Write-Host "  [OK] BUILD SUCCEEDED" -ForegroundColor Green
}
else {
    Write-Host "  [FAIL] BUILD FAILED" -ForegroundColor Red
}

# ------------------------------------------------------------
# SUMMARY
# ------------------------------------------------------------
Write-Host ""
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host " AUDIT COMPLETE" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""

if ($buildExit -eq 0) {
    Write-Host "BUILD STATUS: SUCCESS" -ForegroundColor Green
}
else {
    Write-Host "BUILD STATUS: FAILED" -ForegroundColor Red
}

Write-Host ""
Write-Host "IMPORTANT: No project files were modified by this audit." -ForegroundColor Cyan
Write-Host ""
