# Copilot Instructions

## Project Guidelines
- For CLI boolean flags like force, prefer reading from `Parameters` via `TryGetValue` and checking the parameter's flag property instead of parsing optional string values.
- Prefer using existing framework abstractions/helpers in this codebase (e.g., parameter dictionary flags and operation extensions) instead of manual parsing logic.