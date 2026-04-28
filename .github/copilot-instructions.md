# Copilot Instructions

## Project Guidelines
- Database updater should provide a small built-in command set and support consumer-defined extensions (e.g., seeding, query analyzer) without modifying the original package.
- Do not add redundant service registrations in this codebase's tests when the extension method already handles them.
- When testing mediator extensions in this codebase, prefer narrow regression tests around the extension/pipeline behavior itself and avoid broad tests of unrelated MediatR integration behavior.
- Use the `GenericDefaultExceptionPipeline` as the MediatR fallback to convert unexpected exceptions into clean `UnitResult` responses for the rest of the application to handle.
- Use `IScopedResources`/`DefaultScopedResources` as the scoped bag for heavy resources shared between interceptors and processors, rather than constructor-injecting those heavy scoped dependencies into child components.
- In this codebase, outbox handling intentionally uses two scoped resource channels: DI-injected scoped resources for handler persistence side effects and a separate flow-scoped resources path from repository -> interceptor -> factory/processor.