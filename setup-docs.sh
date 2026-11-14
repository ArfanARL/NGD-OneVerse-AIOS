#!/usr/bin/env bash

set -e

echo "=============================================="
echo " NGD OneVerse AI-OS Documentation Setup"
echo "=============================================="

# Root documentation folder
mkdir -p docs

# ---------------------------------------------------
# Project Management
# ---------------------------------------------------
mkdir -p docs/00-Project-Management

touch docs/00-Project-Management/README.md
touch docs/00-Project-Management/Vision.md
touch docs/00-Project-Management/Scope.md
touch docs/00-Project-Management/Roadmap.md
touch docs/00-Project-Management/Milestones.md
touch docs/00-Project-Management/Risks.md

# ---------------------------------------------------
# Enterprise Architecture
# ---------------------------------------------------
mkdir -p docs/01-Enterprise-Architecture

for i in $(seq -w 1 12)
do
    touch docs/01-Enterprise-Architecture/Volume-${i}.md
done

touch docs/01-Enterprise-Architecture/README.md

# ---------------------------------------------------
# Module Blueprints
# ---------------------------------------------------
mkdir -p docs/02-Module-Blueprints

modules=(
"BP-001-Identity"
"BP-002-Administration"
"BP-003-Configuration"
"BP-004-Audit"
"BP-005-AI-Platform"
"BP-006-AI-Knowledge-Hub"
"BP-007-AI-Agent-Marketplace"
"BP-008-Document-Management"
"BP-009-Workflow-Engine"
"BP-010-Notification-Center"
"BP-011-Reporting"
"BP-012-Integration-Hub"
"BP-013-HR"
"BP-014-CRM"
"BP-015-Sales"
"BP-016-Procurement"
"BP-017-Inventory"
"BP-018-Production"
"BP-019-Finance"
"BP-020-Project-Management"
"BP-021-Customer-Portal"
"BP-022-Vendor-Portal"
"BP-023-Employee-Self-Service"
"BP-024-Enterprise-Analytics"
"BP-025-Mobile-Platform"
"BP-026-Developer-Portal"
"BP-027-Extension-Marketplace"
)

for module in "${modules[@]}"
do
    touch "docs/02-Module-Blueprints/${module}.md"
done

touch docs/02-Module-Blueprints/README.md

# ---------------------------------------------------
# Architecture Decision Records
# ---------------------------------------------------
mkdir -p docs/03-Architecture-Decisions

for i in $(seq -w 1 25)
do
    touch docs/03-Architecture-Decisions/ADR-${i}.md
done

touch docs/03-Architecture-Decisions/README.md

# ---------------------------------------------------
# Database
# ---------------------------------------------------
mkdir -p docs/04-Database

touch docs/04-Database/README.md
touch docs/04-Database/ERD.md
touch docs/04-Database/Schemas.md
touch docs/04-Database/Migrations.md
touch docs/04-Database/Standards.md

# ---------------------------------------------------
# API
# ---------------------------------------------------
mkdir -p docs/05-API/OpenAPI

touch docs/05-API/README.md
touch docs/05-API/REST-Standards.md
touch docs/05-API/Authentication.md
touch docs/05-API/Error-Codes.md

# ---------------------------------------------------
# AI
# ---------------------------------------------------
mkdir -p docs/06-AI

touch docs/06-AI/README.md
touch docs/06-AI/AI-Architecture.md
touch docs/06-AI/Prompt-Library.md
touch docs/06-AI/RAG.md
touch docs/06-AI/Agents.md
touch docs/06-AI/AI-Governance.md

# ---------------------------------------------------
# Remaining Sections
# ---------------------------------------------------
sections=(
"07-Security"
"08-DevOps"
"09-Testing"
"10-UI-UX"
"11-Operations"
"12-User-Guides"
"13-Developer-Guides"
"14-Release-Notes"
"15-Templates"
)

for section in "${sections[@]}"
do
    mkdir -p "docs/${section}"
    touch "docs/${section}/README.md"
done

# ---------------------------------------------------
# Root Files
# ---------------------------------------------------
touch docs/README.md

echo ""
echo "=============================================="
echo " Documentation structure created successfully!"
echo "=============================================="