# Copilot Instructions

## Project Guidelines
- For CLI boolean flags like force, prefer reading from `Parameters` via `TryGetValue` and checking the parameter's flag property instead of parsing optional string values.
- Prefer using existing framework abstractions/helpers in this codebase (e.g., parameter dictionary flags and operation extensions) instead of manual parsing logic.
- Prefer using the host builder's existing configuration pipeline and DI rather than manually rebuilding `IConfiguration`. When custom host wiring makes `HostBuilderContext.Configuration` available only via `ConfigureHostConfiguration`, that approach is acceptable.