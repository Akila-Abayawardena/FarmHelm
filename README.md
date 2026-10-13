# FarmHelm

FarmHelm is a local-first Windows farm management application for managing crop setup, planting batches, plant mortality, crop growth stages, and agricultural production records.

The current release is an Agricultural Core MVP. It is not the complete long-term FarmHelm vision.

## Current Status

Agricultural Core MVP complete and demo-ready.

Implemented:

- Farm setup and Farm Locations
- Crops, Varieties, Crop Growth Stages, and Mortality Reasons
- Planting Batches with optional individual Plant tracking
- Batch mortality and individual Plant mortality
- Stage history and historical Batch removal
- Dashboard
- PostgreSQL persistence
- Windows Tauri desktop application
- Automated backend and frontend CI

## MVP Workflow

```text
Farm
  -> Location / Mortality Reasons
  -> Crop
  -> Variety / Growth Stages
  -> Batch
  -> Stage changes / Mortality
  -> Dashboard
```

Batch removal is a historical lifecycle transition, not deletion.

## Architecture

```text
React / Vite
    |
    v
Tauri Desktop Shell
    |
    v
ASP.NET Core Sidecar
127.0.0.1:5223
    |
    v
PostgreSQL
```

Browser development remains available separately:

```text
React / Vite :5173
    |
    v
Vite proxy
    |
    v
ASP.NET Core :5222
```

FarmHelm is a local-first modular monolith. UUIDs are internal entity keys; human-readable business codes are separate identifiers for users and operations.

## Technology Stack

| Area | Technology |
| --- | --- |
| Desktop | Tauri 2, Rust |
| Frontend | React, TypeScript, Vite, React Router, native fetch, Vitest, React Testing Library |
| Backend | ASP.NET Core, .NET 10, Entity Framework Core, xUnit |
| Database | PostgreSQL |

## Desktop Behavior

The MVP desktop application targets Windows x64. Tauri bundles the React UI and starts the self-contained ASP.NET Core API sidecar on `127.0.0.1:5223`; it also terminates that sidecar when the desktop application closes. The packaged sidecar does not require a separately installed .NET runtime.

PostgreSQL remains an external local prerequisite. The current demo setup relies on the configured machine's FarmHelm PostgreSQL database and ASP.NET User Secret connection string. The NSIS installer is currently unsigned.

## Development Setup

### Browser development

From the repository root, start the API:

```powershell
dotnet run --project .\backend\FarmHelm.Api\FarmHelm.Api.csproj --launch-profile http
```

In another terminal:

```powershell
cd frontend
npm ci
npm run dev
```

The frontend runs at `http://localhost:5173`; the API runs at `http://localhost:5222`. Vite proxies `/api` and `/health` to the API during browser development.

### Desktop development

```powershell
cd frontend
npm ci
npm run desktop:dev
```

This automatically prepares the API sidecar, starts Vite and Tauri, and starts ASP.NET Core on `127.0.0.1:5223`. Do not manually start the API for desktop mode.

### Desktop build

```powershell
cd frontend
npm run desktop:build
```

This produces the Windows x64 NSIS package under `frontend/src-tauri/target/release/bundle/nsis/`. The installer is unsigned and generated installers are not tracked by Git.

## Database Prerequisites

The current desktop MVP requires:

- PostgreSQL installed and running locally
- a FarmHelm database
- the `farmhelm_app` role/configuration
- an ASP.NET User Secret for `ConnectionStrings:FarmHelmDatabase`

EF Core manages migrations. Do not place database passwords or connection strings in frontend configuration.

## Testing and CI

Current validated test counts:

- Backend: 109 tests
- Frontend: 42 tests

GitHub Actions runs on pull requests to `main` and pushes to `main`.

- Backend CI restores, builds, and tests `FarmHelm.sln`.
- Frontend CI runs `npm ci`, typecheck, lint, tests, and a production build.

## Business Data Principles

- Important historical records are retained.
- A Batch's original plant count is immutable.
- Mortality drives derived alive, dead, and survival values.
- Growth Stage is separate from Batch Status.
- Configurable master data is used where appropriate.
- The backend remains authoritative for business rules.

## Project Structure

- `backend/` - ASP.NET Core, Domain, Application, Infrastructure, and tests
- `frontend/` - React/Vite application and frontend tests
- `frontend/src-tauri/` - Tauri desktop shell and sidecar configuration
- `docs/` - architecture, API, and decision records
- `.github/workflows/` - continuous integration workflow

## Future Scope

The following areas are planned and are not part of the Agricultural Core MVP:

- Treatments
- Farm Tasks and scheduling
- Inventory
- Purchasing and Suppliers
- Harvest
- Customers
- Sales and Payments
- Expenses and Finance
- Assets
- Calendar
- Reports
- Search
- Backup and restore
- Authentication and future SaaS concerns

## License

This project currently has no open-source license.
