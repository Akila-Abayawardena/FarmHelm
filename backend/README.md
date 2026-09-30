# FarmHelm Backend

FarmHelm uses a modular-monolith backend built with ASP.NET Core on .NET 10.

## Projects

- **FarmHelm.Domain** contains the core business model and business rules. It must not depend on infrastructure or framework concerns.
- **FarmHelm.Application** contains application use cases and orchestration. It depends on Domain.
- **FarmHelm.Infrastructure** contains technical implementations such as persistence. EF Core and PostgreSQL will live here later. It depends on Application and Domain.
- **FarmHelm.Api** is the HTTP boundary and application host. It depends on Application and Infrastructure.
- **FarmHelm.Tests** contains automated backend tests.

## Dependency Rule

Dependencies point inward: Domain has no FarmHelm project dependencies; Application may depend only on Domain; Infrastructure may depend on Application and Domain; Api may depend on Application and Infrastructure.
