
# IDFCR-modernised
## IDFCR: Intent-Driven Flow Composition Runtime
### _A controlled data execution pipeline with composable behaviour layers_

[//]: # (Badges: replace with your CI / NuGet / coverage badges)
[![Build Status](https://img.shields.io/badge/build-pending-lightgrey)]()
[![NuGet](https://img.shields.io/badge/nuget-latest-lightgrey)]()
[![License](https://img.shields.io/badge/license-MIT-lightgrey)]()

A modern .NET framework that provides composable abstraction layers, persistence infrastructure, AI integration, database tooling, and a CLI-driven build tool. IDFCR is designed around clear separation of concerns so you can compose behaviour (filters, interceptors, mappers, etc.) around domain operations and run them reliably in production or CI pipelines.

Table of contents
- Projects
  - IDFCR.Abstractions
  - IDFCR.AI
  - IDFCR.Persistence.EntityFramework
  - IDFCR.DatabaseUpdater
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

- #### `IDFCR.Abstractions.Mapper`
  Mapping abstractions (`IMapper<TSource>`, `IRecordMapper`, `MapperBase`) for transforming between DTOs, domain entities, and persistence records. Supports parameterised and parameterless mapping so mapping logic stays decoupled from consumers.

- #### `IDFCR.Abstractions.Filters`
  Query filter contracts (`IFilter<TRequest, TDb>`, `FilterBase`, `PagedFilter`) for composing conditional logic against `IQueryable` sources. Keeps query composition separate from repository implementations.

- #### `IDFCR.Abstractions.Interceptors`
  Interceptor framework (`IEntityInterceptor`, `EntityInterceptorBase`, `IEntityInterceptorFactory`) for hooking into lifecycle events (insert, update, delete) at pre/post stages. Useful for soft deletes, auditing, additional validation, or background side-effects.

- #### `IDFCR.Abstractions.Interceptors.DependencyInjection`
  `IServiceCollection` extension helpers that simplify registering interceptors, keeping DI wiring separated from the contracts.

- #### `IDFCR.Abstractions.Persistence`
  Repository and Unit of Work contracts (`IRepository<T, TKey>`, `IUnitOfWork`, `RepositoryBase`) covering CRUD operations, pagination, and integration with filters/interceptors. Implementations plug into these interfaces to provide concrete storage behaviour.

- #### `IDFCR.Abstractions.DatabaseUpdater`
  Contracts for migration management (`IDatabaseFascade`, `ITargetDatabaseConfiguration`) to check for and apply pending migrations in a consistent, implementation-agnostic manner.

- #### `IDFCR.Abstractions.DependencyInjection`
  Core DI utilities built on `Microsoft.Extensions.DependencyInjection` and Scrutor for assembly scanning and service decoration.

- #### `IDFCR.Abstractions.Cli`
  Lightweight CLI abstractions: `IPromptGreeter`, `IArgumentParameters`, command dispatching and CLI application state. The foundation for interactive console apps that the solution includes.

- #### `IDFCR.Abstractions.Cli.Extensions`
  Convenience extension methods for wiring CLI hosts and low-boilerplate hosting patterns.

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

### BuildTools

A self-contained sub-solution providing a CLI-driven build and packaging pipeline. BuildTools consumes published IDFCR NuGet packages to create deterministic build steps and packaging flows.

- #### `BuildTools.Shared`
  Common types and models used by build steps.

- #### `BuildTools.Infrastructure`
  Abstract contracts for build operations so orchestration logic can remain independent of concrete implementations.

- #### `BuildTools.Infrastructure.SqlServer`
  SQL Server-specific build steps that handle database connection and schema operations (used by the build pipeline to run database-related checks or generate artifacts).

- #### `BuildTools.DatabaseUpdater`
  Wraps the database updater as a build step so migrations can be executed during packaging or release workflows.

- #### `BuildTools.Cli`
  The executable entry point for the build pipeline. Hosted with `Microsoft.Extensions.Hosting`, wired with `IDFCR.Abstractions.Cli.Extensions`, and supports user secrets for local configuration.

---

## Technology Stack

| Concern | Library / Version |
|---|---|
| Runtime | .NET 10 |
| ORM | Entity Framework Core 10 |
| DI / Hosting | Microsoft.Extensions.* (v10) |
| Assembly scanning | Scrutor 7 |
| Database | SQL Server (dev via Docker / docker-compose.yml) |
| CI / Build | (CI provider of choice; update badges) |
| AI Providers | HTTP-based provider adapters (OpenAI adapter included) |

---

## Quickstart

Prerequisites
- .NET SDK 10 (install from https://dotnet.microsoft.com)
- Docker (for running SQL Server locally)
- (Optional) An OpenAI API key or other AI provider credentials if you plan to use IDFCR.AI.OpenAI

Clone
- git clone https://github.com/RomeshAbeyawardena/IDFCR-modernised.git
- cd IDFCR-modernised

Build
- dotnet build

Run tests
- dotnet test --no-build

Run a local SQL Server for development (docker)
- docker compose up -d
- By default the repository's docker-compose.yml will spin up a SQL Server container; see docker-compose.yml for environment variables and connection strings.

Apply EF Core migrations locally (example)
- dotnet run --project ./src/IDFCR.DatabaseUpdater -- --connection-string "Server=localhost,1433;Database=idfcr_dev;User Id=sa;Password=Your_password123;"

Run BuildTools CLI (example)
- cd src/BuildTools/BuildTools.Cli
- dotnet run -- [build-step] --arg1 value

Notes:
- Replace connection strings, secrets and API keys with your local development values or use user secrets/environment variables as documented in each project's README or configuration sections.

---

## Local development

Branching and coding workflow
- Use feature branches (e.g., feature/describe-change)
- Keep PRs small and focused
- Follow existing patterns for DI registration, interceptors, and mapping

Common commands
- dotnet watch --project <project> run (for rapid iteration on console projects)
- dotnet ef migrations add <Name> --project ./src/IDFCR.Persistence.EntityFrameworkCore --startup-project ./src/IDFCR.DatabaseUpdater (adjust paths to your layout)
- dotnet pack -c Release (to produce NuGet packages for BuildTools consumption)

Where to find sources
- Browse subprojects under src/ (or the top-level folders listed in Projects). Add links to package docs or subproject READMEs for deeper docs.

---

## Running the database updater & migrations

The DatabaseUpdater console app is intended to be used in automation as well as local development.

Example:
- dotnet run --project ./src/IDFCR.DatabaseUpdater -- --connection-string "<CONN_STRING>" --apply

Common flags (example; confirm actual CLI args in the project)
- --connection-string / -c : target DB connection string
- --apply : apply any pending migrations
- --dry-run : show pending migrations without applying

Always back up production databases before applying migrations in production environments.

---

## Running BuildTools

BuildTools depends on published NuGet packages for deterministic builds. For local development you can:
1. Build and pack the projects you need:
   - dotnet pack -c Release ./src/IDFCR.* -o ./nupkgs
2. Configure BuildTools to use local package source (add ./nupkgs to NuGet.Config or pass as --source)
3. Run BuildTools.Cli:
   - dotnet run --project ./src/BuildTools/BuildTools.Cli -- [pipeline-commands]

Refer to the BuildTools project README (if present) for exact pipeline step names and CLI arguments.

---

## Tests

- Unit tests live alongside the projects under test (or in dedicated test projects). Run:
  - dotnet test ./tests --logger:trx
- Consider running tests in CI using the provided test commands and publishing test results.

---

## Contributing

Contributions are welcome.

Suggested workflow
- Fork the repository
- Create a descriptive branch: feature/<name> or fix/<issue-id>
- Open a pull request against main with a clear description of changes and testing performed
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

Primary maintainer: @RomeshAbeyawardena (maintainership note)
For urgent/troubleshooting contact: open an issue tagged [help wanted] or message the maintainer directly if agreed upon.

---

If you want, I can:
- Create a PR that replaces the current README.md with this content.
- Or produce a shorter "diff-style" patch that restores only missing sections (Quickstart, Tests, Contributing, License).

Which would you prefer?