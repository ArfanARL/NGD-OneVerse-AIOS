# NGD OneVerse AIOS — Architecture Baseline

## Current platform
- .NET 10
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- Solution projects: SharedKernel, Domain, Application, Infrastructure, Api, Tests

## Target modules
1. IAM/RBAC
2. Organization structure
3. Recruitment
4. AI CV screening
5. Interview/selection
6. Onboarding/induction
7. Training
8. Employee master
9. Attendance/leave
10. Payroll/finance
11. Generic workflow/approval engine
12. Documents
13. Notifications
14. Audit/compliance/reporting
15. Policy/compliance RAG

## AI boundary
Keep AI provider-independent:
LLM_PROVIDER=lm_studio|openai
LLM_BASE_URL=http://localhost:1234/v1
LLM_MODEL=qwen2.5-coder-14b
LLM_API_KEY=...

LM Studio is preferred for PII-sensitive processing. Cloud use is optional and must be governed.

## Security
Server-side RBAC, resource/department scoping, short-lived access tokens, refresh rotation, PII protection, rate limiting, append-only audit logs, separate AI interaction logs, grounded-only compliance RAG.

## Delivery
Phase 1 Foundation
Phase 2 Recruitment
Phase 3 Onboarding + Training
Phase 4 Performance + Workflow
Phase 5 Attendance + Leave
Phase 6 Payroll + Finance
Phase 7 AI + Analytics + RAG hardening
