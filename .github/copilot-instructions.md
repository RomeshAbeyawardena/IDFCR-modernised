# Copilot Instructions

## Project Guidelines
- Database updater should provide a small built-in command set and support consumer-defined extensions (e.g., seeding, query analyzer) without modifying the original package.
- Do not add redundant service registrations in this codebase's tests when the extension method already handles them.
- When testing mediator extensions in this codebase, prefer narrow regression tests around the extension/pipeline behavior itself and avoid broad tests of unrelated MediatR integration behavior.
- Use the `GenericDefaultExceptionPipeline` as the MediatR fallback to convert unexpected exceptions into clean `UnitResult` responses for the rest of the application to handle.
- Use `IScopedResources`/`DefaultScopedResources` as the scoped bag for heavy resources shared between interceptors and processors, rather than constructor-injecting those heavy scoped dependencies into child components.
- In this codebase, outbox handling intentionally uses two scoped resource channels: DI-injected scoped resources for handler persistence side effects and a separate flow-scoped resources path from repository -> interceptor -> factory/processor.
- Avoid calling `SaveChangesAsync` prematurely; commits must occur after pipeline/interceptor/processor flow completes to prevent inconsistent state and hard rollback scenarios.
- Ensure high-quality test changes include a multi-pass review before completion.

## Serialization Guidelines
- For result serialization shape, use consumer-provided names for scalar/container values (e.g., Guid as clientId), while keeping POCO results represented as property key-value dictionaries.