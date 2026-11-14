# NGD OneVerse AIOS — Master Blueprint Reference

**Status:** Working product reference  
**Architecture principle:** One unified, modular enterprise platform with shared identity, authorization, organization data, workflows, audit, integrations and AI services.

## Product vision
Build an integrated enterprise operating platform combining organizational management, business applications, institutional knowledge, secure workflows and permission-aware AI. Reuse shared services and data across apps rather than creating disconnected products.

## Blueprint register

| ID | Blueprint | Priority | Scope |
|---|---|---:|---|
| BP-00 | Platform Architecture & Engineering Standards | P0 | Modular architecture, conventions, configuration, logging and observability |
| BP-01 | Identity, Authentication & Access Control | P0 | Users, JWT, roles, permissions, MFA/SSO readiness and identity lifecycle |
| BP-02 | Organization & Enterprise Structure | P0 | Organizations, sites, departments, employees and hierarchy |
| BP-03 | Workflow, Approvals & Business Rules | P0 | Approval chains, delegation, escalation, notifications and rules |
| BP-04 | Audit, Compliance & Governance | P0 | Audit trails, access history, retention and compliance evidence |
| BP-05 | Shared Platform Services & Integration | P0 | Background jobs, events, files, notifications and API integrations |
| BP-06 | CRM & Customer Management | P1 | Leads, contacts, opportunities and pipeline |
| BP-07 | Sales, Quotations & Contracts | P1 | Products, pricing, quotations, orders and contracts |
| BP-08 | Finance, Accounting & Invoicing | P2 | Ledger, invoices, payments, budgets and reports |
| BP-09 | Purchasing & Vendor Management | P1 | Vendors, purchase requests, tenders and purchase orders |
| BP-10 | Inventory, Warehousing & Assets | P1 | Stock, locations, transfers, assets and maintenance |
| BP-11 | HRMS & Workforce Management | P1 | Employee lifecycle, recruitment, onboarding, attendance, leave and appraisals |
| BP-12 | Payroll, Benefits & Expenses | P2 | Payroll, allowances, deductions, expenses and reimbursements |
| BP-13 | Projects, Tasks & Timesheets | P1 | Projects, milestones, tasks, workload and time tracking |
| BP-14 | Manufacturing, Quality & Maintenance | P2 | Production, quality inspections and equipment maintenance |
| BP-15 | Marketing, Campaigns & Events | P2 | Campaigns, marketing automation, events and surveys |
| BP-16 | Website, eCommerce & Customer Portal | P2 | Websites, catalogues, online orders and customer self-service |
| BP-17 | Helpdesk, Field Service & Scheduling | P1 | Tickets, SLAs, field visits, scheduling and resolution |
| BP-18 | Documents, E-Signature & Records | P1 | Repository, versions, approvals, signatures and retention |
| BP-19 | Knowledge Portal & Learning | P1 | Books, chapters, sections, content, policies, courses and search |
| BP-20 | Collaboration & Productivity | P1 | Messaging, announcements, calendars, meetings and workspaces |
| BP-21 | OneVerse AI Assistant | P1 | Permission-aware conversational access to approved enterprise knowledge |
| BP-22 | AI Document Intelligence & RAG | P1 | OCR, parsing, chunking, embeddings, hybrid retrieval and cited answers |
| BP-23 | AI Agent & Tool Orchestration | P2 | Agents, approved tools, human approval gates, logs and recovery |
| BP-24 | Enterprise Search & Knowledge Graph | P2 | Cross-module search, semantic retrieval and entity relationships |
| BP-25 | AI Workflow Automation | P2 | Event-triggered workflows, classification, routing and controlled automation |
| BP-26 | Analytics, BI & Decision Intelligence | P2 | KPIs, dashboards, reports, trends and governed data access |
| BP-27 | AI Model Gateway & Evaluation | P2 | Model routing, local/cloud models, budgets, evaluation and governance |
| BP-28 | Security Intelligence & Risk Monitoring | P2 | Security events, anomaly detection and incident workflows |
| BP-29 | App Studio, Marketplace & Extensibility | P3 | Custom modules, app registry, extensions and developer tooling |

## Architecture and technology direction
- Backend: .NET / ASP.NET Core, EF Core and PostgreSQL.
- Frontend: Next.js, React, TypeScript and Tailwind CSS.
- Start with a modular monolith and explicit domain boundaries; split services only when justified.
- Enforce authorization on the server for every protected operation.
- AI retrieval and tool execution must respect permissions, cite sources where applicable, and audit sensitive actions.
- Use DTO-based APIs, validation, automated tests, reviewed migrations, CI builds and documented architecture decisions.

## Shared enterprise hierarchy
Organization → Site → Department → Employee.

Identity, authorization, audit, notifications and workflows should be shared capabilities, not independently duplicated in every module.

## Implementation sequence
1. Stabilize the existing solution and establish a clean build/test baseline.
2. Finish authentication/RBAC and organization/site/department/employee APIs.
3. Review database mappings and migrations safely; never blindly apply migrations that may recreate existing tables.
4. Add automated tests and CI.
5. Implement Knowledge Portal: Books → Chapters → Sections → Content/Documents.
6. Add permission-aware enterprise search, document intelligence and AI assistant.
7. Expand business apps based on validated requirements and priorities.
8. Add advanced agent orchestration, analytics, model governance and extensibility.

## Blueprint artifact standard
Each blueprint should define purpose, scope, actors, requirements, workflows, data model, API contracts, UI/UX, permissions, audit requirements, test plan, dependencies, acceptance criteria and implementation status. Keep docs synchronized with code.

## Working rules
- Preserve existing work; do not restart the repository to fit this document.
- Inspect actual files and Git state before modifying code.
- Do not overwrite existing files without review.
- Do not store secrets in source control or generated reports.
- Verify every code change with builds and relevant tests.
- Treat this as the product reference; implementation status must be established from the repository, not assumed from the roadmap.
