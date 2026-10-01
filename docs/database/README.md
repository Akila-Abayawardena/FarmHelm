# Database

- PostgreSQL is the selected database.
- EF Core migrations will manage application schema evolution.
- UUIDs will be used as internal primary keys.
- Human-readable codes are separate business identifiers.
- Historical records should generally be retained rather than hard-deleted.
- Detailed schema design will be added incrementally by domain.

## Local Development Database

- Server: `localhost`
- Port: `5432`
- Database: `farmhelm`
- Application role: `farmhelm_app`

The `postgres` role is used only for administrative setup. FarmHelm normally connects using `farmhelm_app`.

Development credentials are stored with ASP.NET Core User Secrets and must never be committed to Git. From the repository root, set the local connection string with PowerShell:

```powershell
dotnet user-secrets set `
  "ConnectionStrings:FarmHelmDatabase" `
  "Host=localhost;Port=5432;Database=farmhelm;Username=farmhelm_app;Password=<LOCAL_PASSWORD>" `
  --project .\backend\FarmHelm.Api\FarmHelm.Api.csproj
```

EF Core migrations will manage schema evolution.

## Agricultural Core Migration

- The first Agricultural Core migration is named `InitialAgriculturalCore`.
- Migration source files live in `backend/FarmHelm.Infrastructure/Persistence/Migrations`.
- Generate migrations through the Infrastructure project with the Api project as the startup project.
- Migrations are source controlled and must be reviewed before they are applied.
- `InitialAgriculturalCore` has been validated against the local PostgreSQL development database.
