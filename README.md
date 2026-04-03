# IDFCR-modernised

A modern .NET framework providing abstraction layers, persistence infrastructure, AI integration, database tooling, and a CLI build tool — designed around clean separation of concerns and composable, interchangeable implementations.

---

## Projects

### IDFCR.Abstractions

The core of the framework. Each project in this group defines contracts, base classes, or lightweight default implementations for a specific concern.

#### `IDFCR.Abstractions.Metadata`
Defines the base identity and audit interfaces shared across the domain: `IIdentifiable`, `IAuditCreatedTimestamp`, `IAuditModifiedTimestamp`, and `ISuppressable`. Any entity that participates in persistence or auditing builds on these.

#### `IDFCR.Abstractions.Results`
Provides a consistent result pattern (`IUnitResult<T>`, `UnitPagedResult<T>`) for communicating operation outcomes — success, failure, exception, failure reason, and metadata — without relying on exceptions as flow control.

#### `IDFCR.Abstractions.Builders`
Fluent builder utilities, primarily `IDictionaryBuilder` / `DictionaryBuilder`, used throughout the framework for structured data construction.

#### `IDFCR.Abstractions.Mapper`
Mapping abstractions (`IMapper<TSource>`, `IRecordMapper`, `MapperBase`) for transforming data between layers. Supports parameterised and parameterless mapping.

#### `IDFCR.Abstractions.Filters`
Query filter contracts (`IFilter<TRequest, TDb>`, `FilterBase`, `PagedFilter`) for composing conditional logic against `IQueryable` sources, keeping filtering decoupled from repository implementations.

#### `IDFCR.Abstractions.Interceptors`
An interceptor framework (`IEntityInterceptor`, `EntityInterceptorBase`, `IEntityInterceptorFactory`) for hooking into entity operations (insert, update, delete) at pre and post stages. Used to implement cross-cutting concerns like auditing and soft-delete.

#### `IDFCR.Abstractions.Interceptors.DependencyInjection`
`IServiceCollection` extension methods for registering interceptors, keeping DI wiring separate from the core interceptor contracts.

#### `IDFCR.Abstractions.Persistence`
Repository and Unit of Work contracts (`IRepository<T, TKey>`, `IUnitOfWork`, `RepositoryBase`) covering CRUD, pagination, and interceptor integration. Implementations plug into these interfaces without consumers needing to care about the underlying storage.

#### `IDFCR.Abstractions.DatabaseUpdater`
Contracts for database migration management (`IDatabaseFascade`, `ITargetDatabaseConfiguration`): checking for and applying pending migrations in a consistent, implementation-agnostic way.

#### `IDFCR.Abstractions.DependencyInjection`
Core DI utilities built on `Microsoft.Extensions.DependencyInjection` and [Scrutor](https://github.com/khellang/Scrutor) for assembly scanning and service decoration.

#### `IDFCR.Abstractions.Cli`
A small CLI framework: `IPromptGreeter` (time-aware greeting generation), `IArgumentParameters`, command dispatching, and CLI application state management. The foundation for any interactive console tool in the solution.

#### `IDFCR.Abstractions.Cli.Extensions`
Convenience extension methods that complement `IDFCR.Abstractions.Cli`, reducing boilerplate when wiring up CLI hosts.

---

### IDFCR.AI

Layered AI service integration, from raw transport up to provider-specific operations.

#### `IDFCR.AI.Abstractions`
Low-level transport contracts: `IAIService`, `IAIServiceConfiguration`, `AIServiceRequest` / `AIServiceResponse`, and `VerifiedConnectionResult`. Everything above this layer is built against these interfaces.

#### `IDFCR.AI.Http`
HTTP implementation of `IAIService` (`HttpAIService`). Handles request serialisation, response deserialisation, and HTTP-level error handling for any HTTP-based AI endpoint.

#### `IDFCR.AI.OpenAI`
OpenAI-specific higher-level service (`IOpenAIService`, `OpenAIService`) that targets the OpenAI Responses API. Provides text generation on top of the HTTP transport layer.

---

### IDFCR.Persistence.EntityFramework

Entity Framework Core implementations of the persistence abstractions.

#### `IDFCR.Persistence.EntityFrameworkCore`
Implements `IRepository<T, TKey>` and `IUnitOfWork` using EF Core 10. Integrates filters, interceptors, and mappers from the abstractions layer, so all cross-cutting concerns flow through consistently regardless of the EF model.

#### `IDFCR.Persistence.EntityFrameworkCore.Extensions`
EF Core-specific extension methods that simplify common operations and reduce repetition when working with the EF implementation.

---

### IDFCR.DatabaseUpdater

A runtime console tool that applies pending EF Core database migrations against a configured target database. Uses `IDFCR.Abstractions.DatabaseUpdater` contracts so the migration strategy remains consistent with any other consumer of those abstractions.

---

### BuildTools

A self-contained sub-solution (`BuildTools.slnx`) providing a CLI-driven build pipeline. It is separate from the main framework and oriented around packaging and infrastructure operations.

#### `BuildTools.Shared`
Common models and utilities shared across the other BuildTools projects. Keeps shared concerns out of any single build step.

#### `BuildTools.Infrastructure`
Abstract infrastructure contracts for build operations. Concrete build steps implement these interfaces, keeping orchestration logic independent of the specific tooling used.

#### `BuildTools.Infrastructure.SqlServer`
SQL Server-specific implementation of the build infrastructure contracts. Handles SQL Server connection and schema operations as part of the build pipeline.

#### `BuildTools.DatabaseUpdater`
Wraps the database updater functionality as a build step, allowing migrations to be run as part of an automated build or packaging workflow.

#### `BuildTools.Cli`
The executable entry point (`console application`) for the build pipeline. Hosted via `Microsoft.Extensions.Hosting`, wired using `IDFCR.Abstractions.Cli.Extensions`, and supports user secrets for local configuration.

---

## Technology Stack

| Concern | Library / Version |
|---|---|
| Runtime | .NET 10 |
| ORM | Entity Framework Core 10 |
| DI / Hosting | `Microsoft.Extensions.*` 10 |
| Assembly scanning | Scrutor 7 |
| Database | SQL Server (Docker via `docker-compose.yml`) |
