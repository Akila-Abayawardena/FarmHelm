# Architecture Overview

FarmHelm's intended high-level architecture is:

```text
Tauri Desktop Shell
       ↓
React + TypeScript UI
       ↓
Local ASP.NET Core API
       ↓
Entity Framework Core
       ↓
PostgreSQL
```

FarmHelm will begin as a modular monolith. Detailed module boundaries will be documented later.

## Backend Dependencies

```text
FarmHelm.Api
├── FarmHelm.Application
└── FarmHelm.Infrastructure
    ├── FarmHelm.Application
    └── FarmHelm.Domain

FarmHelm.Application
└── FarmHelm.Domain

FarmHelm.Domain
└── no FarmHelm project dependencies
```
