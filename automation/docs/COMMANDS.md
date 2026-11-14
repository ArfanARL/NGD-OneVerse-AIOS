# Commands

Full:
```powershell
.\automation\scripts\Invoke-NgdAutomation.ps1 -Task All
```

Build:
```powershell
.\automation\scripts\Invoke-NgdAutomation.ps1 -Task Build
```

Test:
```powershell
.\automation\scripts\Invoke-NgdAutomation.ps1 -Task Test
```

Migrate:
```powershell
.\automation\scripts\Invoke-NgdAutomation.ps1 -Task Migrate
```

Smoke:
```powershell
.\automation\scripts\Invoke-NgdAutomation.ps1 -Task Smoke
```

Start API:
```powershell
.\automation\scripts\Invoke-NgdAutomation.ps1 -Task All -StartApi
```

Infrastructure:
```powershell
docker compose -f .\automation\docker-compose.dev.yml up -d
```
