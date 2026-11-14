# ==============================================================
# NGD OneVerse AI-OS
# Documentation Bootstrap Script
# Version: 1.0
# Author: Muhammad Arfan
# Company: Neural Grid Dynamics (NGD)
# ==============================================================

Write-Host ""
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host " NGD OneVerse AI-OS Documentation Bootstrap" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

$Root = "docs"

#---------------------------------------------------
# Helper Functions
#---------------------------------------------------

function New-Folder {

    param([string]$Path)

    if (!(Test-Path $Path)) {
        New-Item -ItemType Directory -Force -Path $Path | Out-Null
        Write-Host "[DIR ] $Path" -ForegroundColor Green
    }
}

function New-Markdown {

    param([string]$File)

    if (!(Test-Path $File)) {

@"
# $(Split-Path $File -LeafBase)

---

Document Status : Draft

Version : 0.1

Last Updated : $(Get-Date -Format "yyyy-MM-dd")

---
"@ | Set-Content $File

        Write-Host "[FILE] $File" -ForegroundColor Yellow
    }
}

#---------------------------------------------------
# Root
#---------------------------------------------------

New-Folder $Root

#---------------------------------------------------
# Project Management
#---------------------------------------------------

$pm = "$Root\00-Project-Management"

New-Folder $pm

@(
"README.md",
"Vision.md",
"Scope.md",
"Roadmap.md",
"Milestones.md",
"Risks.md"
) | ForEach-Object {

    New-Markdown "$pm\$_"

}

#---------------------------------------------------
# Enterprise Architecture
#---------------------------------------------------

$ea = "$Root\01-Enterprise-Architecture"

New-Folder $ea

1..12 | ForEach-Object{

    $num = "{0:D2}" -f $_

    New-Markdown "$ea\Volume-$num.md"

}

New-Markdown "$ea\README.md"

#---------------------------------------------------
# Module Blueprints
#---------------------------------------------------

$bp = "$Root\02-Module-Blueprints"

New-Folder $bp

$Modules = @(

"BP-001-Identity",
"BP-002-Administration",
"BP-003-Configuration",
"BP-004-Audit",
"BP-005-AI-Platform",
"BP-006-AI-Knowledge-Hub",
"BP-007-AI-Agent-Marketplace",
"BP-008-Document-Management",
"BP-009-Workflow-Engine",
"BP-010-Notification-Center",
"BP-011-Reporting",
"BP-012-Integration-Hub",
"BP-013-HR",
"BP-014-CRM",
"BP-015-Sales",
"BP-016-Procurement",
"BP-017-Inventory",
"BP-018-Production",
"BP-019-Finance",
"BP-020-Project-Management",
"BP-021-Customer-Portal",
"BP-022-Vendor-Portal",
"BP-023-Employee-Self-Service",
"BP-024-Enterprise-Analytics",
"BP-025-Mobile-Platform",
"BP-026-Developer-Portal",
"BP-027-Extension-Marketplace"

)

foreach($Module in $Modules){

    New-Markdown "$bp\$Module.md"

}

New-Markdown "$bp\README.md"

#---------------------------------------------------
# ADR
#---------------------------------------------------

$adr="$Root\03-Architecture-Decisions"

New-Folder $adr

1..25 | ForEach-Object{

    $num="{0:D3}" -f $_

    New-Markdown "$adr\ADR-$num.md"

}

New-Markdown "$adr\README.md"

#---------------------------------------------------
# Database
#---------------------------------------------------

$db="$Root\04-Database"

New-Folder $db

@(
"README.md",
"ERD.md",
"Schemas.md",
"Migrations.md",
"Standards.md"
) | ForEach-Object{

    New-Markdown "$db\$_"

}

#---------------------------------------------------
# API
#---------------------------------------------------

$api="$Root\05-API"

New-Folder $api
New-Folder "$api\OpenAPI"

@(
"README.md",
"REST-Standards.md",
"Authentication.md",
"Error-Codes.md"
) | ForEach-Object{

    New-Markdown "$api\$_"

}

#---------------------------------------------------
# AI
#---------------------------------------------------

$ai="$Root\06-AI"

New-Folder $ai

@(
"README.md",
"AI-Architecture.md",
"Prompt-Library.md",
"RAG.md",
"Agents.md",
"AI-Governance.md"
) | ForEach-Object{

    New-Markdown "$ai\$_"

}

#---------------------------------------------------
# Remaining Sections
#---------------------------------------------------

$Sections=@(

"07-Security",
"08-DevOps",
"09-Testing",
"10-UI-UX",
"11-Operations",
"12-User-Guides",
"13-Developer-Guides",
"14-Release-Notes",
"15-Templates"

)

foreach($Section in $Sections){

    $Folder="$Root\$Section"

    New-Folder $Folder

    New-Markdown "$Folder\README.md"

}

#---------------------------------------------------
# Root README
#---------------------------------------------------

New-Markdown "$Root\README.md"

Write-Host ""
Write-Host "===============================================" -ForegroundColor Green
Write-Host " Documentation Bootstrap Completed Successfully" -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Green
Write-Host ""