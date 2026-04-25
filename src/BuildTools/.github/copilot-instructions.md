# Copilot Instructions

## Project Guidelines
- For CLI boolean flags like force, prefer reading from `Parameters` via `TryGetValue` and checking the parameter's flag property instead of parsing optional string values.
- Prefer using existing framework abstractions/helpers in this codebase (e.g., parameter dictionary flags and operation extensions) instead of manual parsing logic.
- Prefer using the host builder's existing configuration pipeline and DI rather than manually rebuilding `IConfiguration`. When custom host wiring makes `HostBuilderContext.Configuration` available only via `ConfigureHostConfiguration`, that approach is acceptable.
- Use MediatR queries to streamline CRUD operations, ensuring that gRPC and the application layer go through the same pipes for consistency. Align tests with this pattern.
- For setting audit evolution, consider using JSON diff tooling to aggregate and analyze change history from `OldValueJson` and `NewValueJson`.