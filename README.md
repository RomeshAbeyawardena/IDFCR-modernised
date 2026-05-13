
# IDFCR-modernised
## IDFCR: Intent-Driven Flow Composition Runtime
### _A controlled data execution pipeline with composable behaviour layers_

[//]: # (Badges: replace with your CI / NuGet / coverage badges)
[![Build Status](https://img.shields.io/badge/build-pending-lightgrey)]()
[![NuGet](https://img.shields.io/badge/nuget-latest-lightgrey)]()
[![License](https://img.shields.io/badge/license-MIT-lightgrey)]()

A modern .NET framework that provides composable abstraction layers, persistence infrastructure, AI integration, gRPC support, database tooling, and a CLI-driven build tool. IDFCR is designed around clear separation of concerns so you can compose behaviour (filters, interceptors, mappers, MediatR pipelines, etc.) around domain operations and run them reliably in production or CI pipelines.

Table of contents
- Projects
  - IDFCR.Abstractions
  - IDFCR.AI
  - IDFCR.Persistence.EntityFramework
  - IDFCR.DatabaseUpdater
  - IDFCR.Results.Http
  - IDFCR.TestUtilities
  - BuildTools
- Technology Stack
- Quickstart
- Local development
- Running the database updater & migrations
- Running BuildTools
- Tests
- Contributing
- License
- Support / Reporting issues
- Maintainers / Contact

---

## Projects

### IDFCR.Abstractions

The core contracts and lightweight base implementations the rest of the solution builds on. These projects define domain and cross-cutting interfaces so implementations remain swappable and testable.

- #### `IDFCR.Abstractions.Metadata`
  Defines standard identity and audit interfaces used across entities: `IIdentifiable`, `IAuditCreatedTimestamp`, `IAuditModifiedTimestamp`, and `ISuppressable`. Use these on domain types to enable standard persistence behaviour and consistent audit tracking.

- #### `IDFCR.Abstractions.Results`
  A consistent result pattern for operations (`IUnitResult<T>`, `UnitPagedResult<T>`) that carries success/failure state, error information, exceptions, and optional metadata (paging, timings). This decouples methods from throwing exceptions for control flow.

- #### `IDFCR.Abstractions.Builders`
  Fluent builder utilities (e.g., `IDictionaryBuilder` / `DictionaryBuilder`) used to construct structured data or parameter dictionaries in a readable and testable way.

- #### `IDFCR.Abstractions.Caching`
  Thread-safe string caching service (`CachedStringService`) backed by a `SemaphoreSlim`. Useful for caching computed strings (e.g., status page content) that are expensive to rebuild on every request.

- #### `IDFCR.Abstractions.Mapper`
  Mapping abstractions (`IMapper<TSource>`, `IRecordMapper`, `MapperBase`) for transforming between DTOs, domain entities, and persistence records. Supports parameterised and parameterless mapping so mapping logic stays decoupled from consumers.

- #### `IDFCR.Abstractions.Mapper.Extensions`
  Provides `AutoMapperBase<TSource>`, a base class that uses FastMember to automatically copy matching properties from source to target. Useful when a simple property-by-property copy is sufficient, eliminating boilerplate mapping code.

- #### `IDFCR.Abstractions.Filters`
  Query filter contracts (`IFilter<TRequest, TDb>`, `FilterBase`, `PagedFilter`) for composing conditional logic against `IQueryable` sources. Keeps query composition separate from repository implementations.

- #### `IDFCR.Abstractions.Interceptors`
  Interceptor framework (`IEntityInterceptor`, `EntityInterceptorBase`, `IEntityInterceptorFactory`) for hooking into lifecycle events (insert, update, delete) at pre/post stages. Useful for soft deletes, auditing, additional validation, or background side-effects.

- #### `IDFCR.Abstractions.Interceptors.DependencyInjection`
  `IServiceCollection` extension helpers that simplify registering interceptors, keeping DI wiring separated from the contracts.

- #### `IDFCR.Abstractions.Interceptors.Extensions`
  Extension methods for `IAuditProcessor` that generate human-readable change descriptions by comparing old and new entity snapshots. Uses FastMember for efficient property reflection and respects `[DisplayName]` and `[IgnoreAuditing]` attributes.

- #### `IDFCR.Abstractions.Persistence`
  Repository and Unit of Work contracts (`IRepository<T, TKey>`, `IUnitOfWork`, `RepositoryBase`) covering CRUD operations, pagination, and integration with filters/interceptors. Implementations plug into these interfaces to provide concrete storage behaviour.

- #### `IDFCR.Abstractions.DatabaseUpdater`
  Contracts for migration management (`IDatabaseFascade`, `ITargetDatabaseConfiguration`) to check for and apply pending migrations in a consistent, implementation-agnostic manner.

- #### `IDFCR.Abstractions.DependencyInjection`
  Core DI utilities built on `Microsoft.Extensions.DependencyInjection` and Scrutor for assembly scanning and service decoration.

- #### `IDFCR.Abstractions.Mediator`
  MediatR request marker interfaces (`IUnitResultRequest`, `IUnitResultRequest<T>`, `IUnitResultCollectionRequest<T>`, `IUnitPagedResultRequest<T>`) that tie MediatR requests directly into the framework's result pattern. Use these on command/query objects to keep the result contract consistent across the pipeline.

- #### `IDFCR.Abstractions.Mediator.Extensions`
  Exception handling infrastructure for MediatR pipelines. `GenericDefaultExceptionPipeline<TRequest, TResponse, TException>` catches exceptions and converts them into the appropriate `IUnitResult` / `IUnitPagedResult` / `IUnitResultCollection` response, driven by a configurable `IExceptionBehaviourManager`. Register it as a fallback `IRequestExceptionHandler` to ensure unhandled exceptions never leak outside the pipeline.

- #### `IDFCR.Abstractions.Cli`
  Lightweight CLI abstractions: `IPromptGreeter`, `IArgumentParameters`, command dispatching and CLI application state. The foundation for interactive console apps that the solution includes.

- #### `IDFCR.Abstractions.Cli.Extensions`
  Convenience extension methods for wiring CLI hosts and low-boilerplate hosting patterns.

- #### `IDFCR.Abstractions.gRPC`
  Service-discovery infrastructure for gRPC. Decorate concrete gRPC service implementations with `[RegisteredGRPCServiceImplementation]` and use `IRegisteredGRPCServiceImplementationTypeDiscoveryService` to scan assemblies at startup, optionally filtering by configuration keys.

- #### `IDFCR.Abstractions.gRPC.Contracts`
  Shared Protobuf (`.proto`) definitions for common gRPC message types (e.g., `UnitResult`, `UnitAction`, `FailureReason`) used across gRPC service contracts in the solution.

- #### `IDFCR.Abstractions.gRPC.Extensions`
  Conversion helpers (`UnitResultExtensions`) for translating between gRPC contract types and the framework's `IUnitResult` types in both directions.

- #### `IDFCR.Abstractions.gRPC.Generated`
  Pre-generated gRPC client/server stubs produced from the shared `.proto` files. Keeps generated code isolated from hand-written source.

- #### `IDFCR.Abstractions.gRPC.HostExtensions`
  `WebApplication` extension (`DiscoverGRPCServices`) that scans the provided assemblies for types decorated with `[RegisteredGRPCServiceImplementation]` and calls `MapGrpcService<T>` for each, removing the need to manually register every gRPC service implementation in `Program.cs`.

---

### IDFCR.AI

Layered AI integration built from low-level transports up to provider-specific helpers. Designed so higher-level components depend only on abstractions and can be swapped between providers.

- #### `IDFCR.AI.Abstractions`
  Low-level transport contracts: `IAIService`, `IAIServiceConfiguration`, `AIServiceRequest`, `AIServiceResponse`, and `VerifiedConnectionResult`. Implementations should conform to these to allow consistent retrying, error handling, and telemetry.

- #### `IDFCR.AI.Http`
  HTTP-based `IAIService` implementation (e.g., `HttpAIService`) handling request serialization, response deserialization, and HTTP-level error handling for any HTTP-based AI endpoint.

- #### `IDFCR.AI.OpenAI`
  Higher-level service targeting OpenAI Responses API (`IOpenAIService`, `OpenAIService`) and convenience wrappers for text generation, prompts, and response parsing. This layer normalizes provider-specific features into the framework's result patterns.

---

### IDFCR.Persistence.EntityFramework

Entity Framework Core implementations of the persistence abstractions. Provides a tested, production-ready EF Core integration.

- #### `IDFCR.Persistence.EntityFrameworkCore`
  Implements `IRepository<T, TKey>` and `IUnitOfWork` using EF Core 10. Integrates filters, interceptors, mappers and maintains consistent behaviour across persistence operations.

- #### `IDFCR.Persistence.EntityFrameworkCore.Extensions`
  EF Core-specific extension methods to simplify common patterns and reduce repetition when working with EF.

---

### IDFCR.DatabaseUpdater

A runtime console tool that applies pending EF Core migrations against a configured target database. Uses the `IDFCR.Abstractions.DatabaseUpdater` contracts so migration strategy remains pluggable and consistent across environments.

Common use-cases:
- Apply migrations as part of deployment pipelines
- Run migrations locally against a dev/test container
- Validate that target database schema is up-to-date in CI

---

### IDFCR.Results.Http

ASP.NET Core result adapters that bridge the framework's `IUnitResult` types to HTTP responses. `IUnitHttpResult` implements `IResult` and maps each `FailureReason` to the appropriate HTTP status code (e.g., `ValidationError` → 400, `NotFound` → 404, `InternalError` → 500). Return `IUnitHttpResult` directly from minimal API endpoints to get consistent, structured JSON error responses without any additional mapping.

---

### IDFCR.TestUtilities

Shared helpers for unit and integration tests across the solution.

- **`InternalMemoryMockRepository<TCommon, TDb, T>`** — An in-memory `RepositoryBase` implementation backed by a `List<TDb>`. Supports CRUD, paging via `IFilterFactory`, and interceptor wiring through `IEntityInterceptorFactory`. Use it in tests that need a real repository contract without a database.
- **`DbContextMarker<TDb>`** — Scoped resource token used to share the in-memory entry list between interceptors and the mock repository.
- **`StringReadableStream` / `StringWriteableStream`** — Stream wrappers around strings for testing CLI managed-stream interactions.

---

### BuildTools

A self-contained sub-solution providing a CLI-driven build and packaging pipeline. BuildTools consumes published IDFCR NuGet packages to create deterministic build steps and packaging flows.

See [src/BuildTools/Readme.md](src/BuildTools/Readme.md) for full setup instructions, user-secrets configuration, CLI command reference, and database schema details.

#### CLI projects

- #### `BuildTools.Shared`
  Common DTOs and query models shared across layers.

- #### `BuildTools.Infrastructure`
  Abstract repository contracts and `DbSettings` configuration model so orchestration logic stays independent of concrete implementations.

- #### `BuildTools.Infrastructure.SqlServer`
  EF Core 10 / SQL Server implementations of all repositories; owns EF Core migrations for the BuildTools schema.

- #### `BuildTools.DatabaseUpdater`
  Console app — applies pending EF Core migrations against the BuildTools target database.

- #### `BuildTools.Cli`
  Interactive CLI console app — manages environments, settings, and packages stored in a SQL Server database. Hosted with `Microsoft.Extensions.Hosting`, wired with `IDFCR.Abstractions.Cli.Extensions`, and supports user secrets for local configuration.

#### Backend / gRPC projects

- #### `BuildTools.Application`
  MediatR-based application layer containing command/query handlers and feature logic for the BuildTools domain.

- #### `BuildTools.Shared.Contracts`
  Shared domain contracts (request/response types) consumed by both the CLI and gRPC layers.

- #### `BuildTools.gRPC.Shared.Contracts`
  Protobuf (`.proto`) definitions for the BuildTools gRPC service contracts.

- #### `BuildTools.gRPC.Application`
  gRPC application services that delegate to the MediatR-based application layer.

- #### `BuildTools.gRPC.Api`
  ASP.NET Core gRPC server entry point. Uses `DiscoverGRPCServices` to auto-register gRPC service implementations and exposes a status endpoint at `/`.

---

## Technology Stack

| Concern | Library / Version |
|---|---|
| Runtime | .NET 10 |
| ORM | Entity Framework Core 10 |
| DI / Hosting | Microsoft.Extensions.* (v10) |
| Assembly scanning | Scrutor 7 |
| Mediator | MediatR |
| gRPC | Grpc.AspNetCore |
| Property reflection | FastMember |
| Database | SQL Server (dev via Docker / docker-compose.yml) |
| AI Providers | HTTP-based provider adapters (OpenAI adapter included) |

---

## Quickstart

Prerequisites
- .NET SDK 10 (install from https://dotnet.microsoft.com)
- Docker (for running SQL Server locally)
- (Optional) An OpenAI API key or other AI provider credentials if you plan to use IDFCR.AI.OpenAI

Clone
```
git clone https://github.com/RomeshAbeyawardena/IDFCR-modernised.git
cd IDFCR-modernised
```

Build
```
dotnet build ./src/src.sln
```

Run tests
```
dotnet test ./src/src.sln --no-build
```

Run a local SQL Server for development (docker)
```
docker compose up -d
```
By default the repository's `docker-compose.yml` will spin up a SQL Server container; see `docker-compose.yml` for environment variables and connection strings.

Apply EF Core migrations locally (example)
```
dotnet run --project ./src/IDFCR.DatabaseUpdater
```

Run BuildTools CLI (example)
```
cd src/BuildTools/BuildTools.Cli
dotnet run -- [command]
```

Notes:
- Replace connection strings, secrets and API keys with your local development values or use user secrets/environment variables as documented in each project's README or configuration sections.

---

## Local development

Branching and coding workflow
- Use feature branches (e.g., `feature/describe-change`)
- Keep PRs small and focused
- Follow existing patterns for DI registration, interceptors, and mapping

Common commands
```
# Watch mode for rapid iteration on console projects
dotnet watch --project <project-path> run

# Add a new EF Core migration for the main persistence project
dotnet ef migrations add <Name> \
  --project ./src/IDFCR.Persistence.EntityFramework/IDFCR.Persistence.EntityFrameworkCore \
  --startup-project ./src/IDFCR.DatabaseUpdater

# Pack for local NuGet consumption
dotnet pack -c Release
```

Where to find sources
- Browse subprojects under `src/` (folders listed in Projects above).
- BuildTools has its own sub-solution at `src/BuildTools/BuildTools.slnx` and its own `Readme.md`.

---

## Running the database updater & migrations

The DatabaseUpdater console app is intended to be used in automation as well as local development.

Example:
```
dotnet run --project ./src/IDFCR.DatabaseUpdater
```

Connection details are loaded from user secrets or environment variables (see `src/IDFCR.DatabaseUpdater` for configuration). Always back up production databases before applying migrations in production environments.

---

## Running BuildTools

BuildTools has two entry points:

### CLI (`BuildTools.Cli`)

See [src/BuildTools/Readme.md](src/BuildTools/Readme.md) for full setup including user secrets, CLI command reference (environment / setting / package commands), and database schema details.

Quick start:
```
cd src/BuildTools/BuildTools.Cli
dotnet user-secrets init
# Populate secrets — see src/BuildTools/Readme.md for the full secrets schema
dotnet run
```

### gRPC API (`BuildTools.gRPC.Api`)

```
cd src/BuildTools/Backend/BuildTools.gRPC.Api
dotnet user-secrets init
dotnet run
```

The gRPC server auto-discovers and registers all `[RegisteredGRPCServiceImplementation]`-decorated service implementations at startup.

---

## Tests

Tests live alongside each project under their respective `Tests/` subfolder. Run all tests from the repository root:

```
dotnet test ./src/src.sln --no-build
```

To run only tests for a specific area:
```
# All Abstractions tests
dotnet test ./src/IDFCR.Abstractions/Tests --no-build

# All AI tests
dotnet test ./src/IDFCR.AI/Tests --no-build

# BuildTools CLI tests
dotnet test ./src/BuildTools/BuildTools.Cli.Tests --no-build
```

Consider publishing results in CI with `--logger trx --results-directory ./TestResults`.

---

## Contributing

Contributions are welcome.

Suggested workflow
- Fork the repository
- Create a descriptive branch: `feature/<name>` or `fix/<issue-id>`
- Open a pull request against `main` with a clear description of changes and testing performed
- Ensure all tests pass and linting/style checks are green

Please include:
- A short description of the change
- The motivation / what problem it fixes
- How to verify the change locally (commands / expected results)

If you have a CONTRIBUTING.md file in the repo, follow the instructions there. If not, open an issue to propose contribution guidelines.

---

## License

See the LICENSE file in the repository root. If there is no license yet, add one (MIT / Apache-2.0 recommended for community projects) and update badges accordingly.

---

## Support / Reporting issues

To report bugs or request features:
- Open an issue in the repository and include:
  - A clear title
  - Steps to reproduce
  - Expected and actual behaviour
  - Logs or stack traces, where relevant
  - Environment details (OS, .NET SDK version, DB version)

For pull requests, reference the issue number where applicable.

---

## Maintainers / Contact

Primary maintainer: @RomeshAbeyawardena
For urgent/troubleshooting contact: open an issue tagged `[help wanted]` or message the maintainer directly if agreed upon.