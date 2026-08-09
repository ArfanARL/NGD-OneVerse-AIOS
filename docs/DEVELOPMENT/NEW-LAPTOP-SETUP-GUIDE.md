\# NGD OneVerse AIOS — New Laptop Setup \& Recovery Guide



\## 1. Project



Project:

NGD-OneVerse-AIOS



Root:

D:\\Projects\\NGD-OneVerse-AIOS



Backend:

D:\\Projects\\NGD-OneVerse-AIOS\\backend



Solution:

backend\\NGD.OneVerse.slnx



\---



\## 2. Required Software



Install and verify:



\- Windows

\- Git

\- .NET 10 SDK

\- PostgreSQL 18

\- pgAdmin 4 (optional)

\- Visual Studio Code

\- PowerShell

\- Docker Desktop (if required by deployment)

\- LM Studio (if required by local AI services)



\---



\## 3. Verify .NET



Run:



dotnet --version



Expected:



10.0.x



Verify:



dotnet --info



Verify executable:



where.exe dotnet



Expected:



C:\\Program Files\\dotnet\\dotnet.exe



\---



\## 4. Verify Git



Run:



git --version



Then:



git status



Confirm the repository and branch are correct.



\---



\## 5. Project Structure



Expected top-level directories:



.github

backend

database

deployment

docs

frontend

infrastructure

shared



Expected backend projects:



NGD.OneVerse.Api

NGD.OneVerse.Application

NGD.OneVerse.Domain

NGD.OneVerse.Infrastructure

NGD.OneVerse.SharedKernel

NGD.OneVerse.Tests



\---



\## 6. PostgreSQL



PostgreSQL version used:



18.x



Executable:



C:\\Program Files\\PostgreSQL\\18\\bin\\psql.exe



Verify:



\& "C:\\Program Files\\PostgreSQL\\18\\bin\\psql.exe" --version



\---



\## 7. Database



Database name:



ngd\_oneverse



Server:



localhost



Port:



5432



User:



postgres



Connection configuration is stored in:



backend\\NGD.OneVerse.Api\\appsettings.Development.json



Default connection:



Host=localhost;Port=5432;Database=ngd\_oneverse;Username=postgres;...



\---



\## 8. Verify Database



Connect:



\& "C:\\Program Files\\PostgreSQL\\18\\bin\\psql.exe" -h localhost -p 5432 -U postgres -d ngd\_oneverse



Inside psql:



\\dt



Verify tables exist.



Check EF migration history:



SELECT \* FROM "\_\_EFMigrationsHistory";



Exit:



\\q



\---



\## 9. Restore .NET Packages



From project root:



dotnet restore .\\backend\\NGD.OneVerse.slnx



Wait for restore to complete.



\---



\## 10. Verify Backend Projects



Run:



Get-ChildItem .\\backend -Recurse -Filter \*.csproj |

Select-Object FullName



Verify all six projects exist.



\---



\## 11. Build Solution



Run:



dotnet build .\\backend\\NGD.OneVerse.slnx --no-restore



Expected:



Build succeeded.



If packages have not yet been restored:



dotnet build .\\backend\\NGD.OneVerse.slnx



\---



\## 12. Verify Tests



Run:



dotnet test .\\backend\\NGD.OneVerse.slnx --no-build



If required:



dotnet test .\\backend\\NGD.OneVerse.slnx



\---



\## 13. EF Core CLI



Verify:



dotnet ef --version



If missing:



dotnet tool install --global dotnet-ef --version 10.\*



If already installed:



dotnet tool update --global dotnet-ef --version 10.\*



\---



\## 14. Verify Existing Migrations



Run:



dotnet ef migrations list --project .\\backend\\NGD.OneVerse.Infrastructure\\NGD.OneVerse.Infrastructure.csproj --startup-project .\\backend\\NGD.OneVerse.Api\\NGD.OneVerse.Api.csproj



Verify the expected migration exists.



Current migration:



20260805182346\_InitialIdentitySchema



\---



\## 15. Database Migration



ONLY run database update when the target database is intentionally empty

or when the migration status has been verified.



Command:



dotnet ef database update --project .\\backend\\NGD.OneVerse.Infrastructure\\NGD.OneVerse.Infrastructure.csproj --startup-project .\\backend\\NGD.OneVerse.Api\\NGD.OneVerse.Api.csproj



Verify:



Applying migration '20260805182346\_InitialIdentitySchema'.



Done.



DO NOT create a duplicate migration if the existing migration is already present.



\---



\## 16. Program.cs



API Program.cs:



backend\\NGD.OneVerse.Api\\Program.cs



The application uses:



\- ASP.NET Core

\- PostgreSQL

\- Entity Framework Core

\- Swagger/OpenAPI



Verify the file contains only one application configuration.



Do not accidentally append a second Program.cs to the existing file.



\---



\## 17. Start API



Run:



dotnet run --project .\\backend\\NGD.OneVerse.Api\\NGD.OneVerse.Api.csproj



Expected:



Now listening on: http://localhost:5187



Application started.



Environment:



Development



\---



\## 18. Swagger



Open:



http://localhost:5187/swagger/index.html



Swagger should display the available API endpoints.



If the root URL returns:



404 Not Found



this does not necessarily indicate an API failure.



The important test is:



/swagger/index.html



\---



\## 19. API Connectivity Test



Test the API from another PowerShell window:



Invoke-WebRequest http://localhost:5187 -UseBasicParsing



A 404 at the root can be normal if no root endpoint is defined.



Test Swagger:



Invoke-WebRequest http://localhost:5187/swagger/index.html -UseBasicParsing



\---



\## 20. PostgreSQL PATH



If psql is not recognized:



where.exe psql



If not found, verify:



C:\\Program Files\\PostgreSQL\\18\\bin\\psql.exe



Use the full path when necessary:



\& "C:\\Program Files\\PostgreSQL\\18\\bin\\psql.exe" --version



\---



\## 21. Common Issues



\### dotnet not recognized



Run:



where.exe dotnet



Expected:



C:\\Program Files\\dotnet\\dotnet.exe



Check:



$env:Path



\---



\### No .NET SDKs found



Run:



dotnet --info



Install the required .NET SDK.



\---



\### dotnet ef not found



Run:



dotnet tool list --global



Then install/update:



dotnet tool install --global dotnet-ef --version 10.\*



\---



\### Build failed



Run:



dotnet restore .\\backend\\NGD.OneVerse.slnx



Then:



dotnet build .\\backend\\NGD.OneVerse.slnx



Read the first actual compiler error.



\---



\### PostgreSQL database exists but has no tables



Verify:



\\dt



Then check migrations:



dotnet ef migrations list --project .\\backend\\NGD.OneVerse.Infrastructure\\NGD.OneVerse.Infrastructure.csproj --startup-project .\\backend\\NGD.OneVerse.Api\\NGD.OneVerse.Api.csproj



If the expected migration is pending, apply it only after confirming the database is the correct target.



\---



\### Swagger returns 404



Verify Program.cs contains:



builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();



and in Development:



app.UseSwagger();

app.UseSwaggerUI();



Then rebuild and restart the API.



\---



\## 22. Security Warning



Current known warning:



NU1903



Package:



Microsoft.OpenApi 2.0.0



The package has a reported high-severity vulnerability.



Do not blindly change the package while troubleshooting a working environment.



Handle dependency updates as a separate controlled task.



\---



\## 23. Final New-Laptop Checklist



\[ ] Windows installed

\[ ] Git installed

\[ ] .NET SDK installed

\[ ] PostgreSQL installed

\[ ] Project copied/cloned

\[ ] Correct project directory confirmed

\[ ] Git status verified

\[ ] PostgreSQL service running

\[ ] ngd\_oneverse database exists

\[ ] Database connection verified

\[ ] Database tables verified

\[ ] NuGet restore successful

\[ ] Solution builds

\[ ] Tests pass

\[ ] dotnet-ef available

\[ ] Existing EF migrations verified

\[ ] Database migration applied if required

\[ ] API starts

\[ ] Swagger opens

\[ ] API/database connectivity verified



\---



\## 24. Current Known Working Environment



Verified on the replacement laptop:



.NET SDK:

10.0.302



PostgreSQL:

18.4



Database:

ngd\_oneverse



API:

http://localhost:5187



Swagger:

http://localhost:5187/swagger/index.html



Current EF migration:



20260805182346\_InitialIdentitySchema



Status:



WORKING

