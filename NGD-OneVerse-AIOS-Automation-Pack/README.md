# NGD OneVerse AIOS — Automation Pack

This automation layer targets the CURRENT working .NET 10 / ASP.NET Core / EF Core / PostgreSQL repository.

The larger HRMS blueprint calls for FastAPI/Python for AI services, so the safe architecture is:
- .NET 10 = core transactional API and system of record
- PostgreSQL = primary database
- Redis + MinIO = infrastructure dependencies
- AI/RAG = future sidecar with LM Studio/OpenAI-compatible provider abstraction
- GitHub Actions + PowerShell = automated build/test/migration/smoke pipeline

Quick start from D:\Projects\NGD-OneVerse-AIOS:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
.\automation\scripts\Invoke-NgdAutomation.ps1 -Task All
```

If the API is already running:
```powershell
.\automation\scripts\Invoke-NgdAutomation.ps1 -Task Smoke
```
