# InsureFlow — Phase 1 Backend Foundation

## What was implemented

- EF Core `InsureFlowDbContext`
- MySQL EF Core provider (`MySql.EntityFrameworkCore` 10.0.9)
- Centralized database configuration
- Application DI extension
- Infrastructure DI extension
- Serilog request/application logging
- Correlation ID (`X-Correlation-ID`)
- Global exception handling
- RFC 7807-style ProblemDetails responses
- API versioning foundation
- Swagger/OpenAPI
- `/health` and `/health/ready`
- EF Core database health check
- Audit abstraction and initial logging implementation

## Database decision

The supplied baseline is MySQL 8.0.46. This implementation therefore uses MySQL instead of silently migrating the existing database to SQL Server.

Do not run `dotnet ef database drop` and do not recreate the existing database.

## Configure the connection string

Preferred for local development: use an environment variable or user-secrets rather than committing credentials.

PowerShell example:

```powershell
$env:ConnectionStrings__InsureFlow="server=localhost;port=3306;database=insureflow_db;user=root;password=YOUR_PASSWORD;"
```

Then run:

```powershell
dotnet restore InsureFlow.slnx
dotnet build InsureFlow.slnx
dotnet run --project backend\InsureFlow.Api
```

Check:

```text
https://localhost:<port>/swagger
https://localhost:<port>/health
https://localhost:<port>/health/ready
```

`/health/ready` includes the EF Core database check.

## Important

Phase 1 intentionally does not implement Patients, Insurance, Eligibility, Authorizations, Claims, Denials, Appeals, or Payments. Those are later feature phases.

The audit service currently provides the audit abstraction and PHI-conscious structured logging foundation. Persistent audit storage should be added when the audit schema/infrastructure is implemented in the SaaS foundation phase.
