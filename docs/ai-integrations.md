# AI integrations

IDFCR provides a thin set of AI abstractions and provider implementations, following the same opt-in, contract-first approach as the rest of the framework.

**Packages:** `IDFCR.AI.Abstractions`, `IDFCR.AI.Http`, `IDFCR.AI.OpenAI`

---

## Abstractions

### IAIService

`IAIService` is the top-level contract for an AI provider. It exposes:

- `VerifyConnectionAsync` — checks that the configured provider is reachable and authenticated.
- Configuration access via `IAIServiceConfiguration`.

```csharp
public interface IAIService
{
    IAIServiceConfiguration Configuration { get; }
    Task<VerifiedConnectionResult> VerifyConnectionAsync(CancellationToken cancellationToken);
}
```

### ITextGeneration

`ITextGeneration` extends `IAIService` with text generation:

```csharp
public interface ITextGeneration : IAIService
{
    Task<AIServiceResponse> GenerateTextAsync(AIServiceRequest request, CancellationToken cancellationToken);
}
```

`AIServiceRequest` carries the prompt and any generation parameters. `AIServiceResponse` returns the generated text and any relevant metadata.

---

## HTTP provider

`IDFCR.AI.Http` provides `HttpAIService`, a generic HTTP-backed implementation that routes to any AI endpoint that accepts JSON.

### Registration

```csharp
using IDFCR.AI.Http.Extensions;

services.AddHttpAIService(configuration);
```

The extension method binds `HttpAIServiceConfiguration` from configuration and registers `ITextGeneration` as `HttpAIService`.

### Configuration

```json
{
  "AIService": {
    "BaseUrl": "https://api.example-ai.com",
    "ApiKey": "...",
    "Model": "text-generation-model"
  }
}
```

---

## OpenAI provider

`IDFCR.AI.OpenAI` provides `OpenAIService`, backed by the OpenAI API.

### Registration

```csharp
using IDFCR.AI.OpenAI.Extensions;

services.AddOpenAIService(configuration);
```

### Configuration

```json
{
  "OpenAI": {
    "ApiKey": "sk-...",
    "Model": "gpt-4o"
  }
}
```

`IOpenAIService` extends `ITextGeneration` with any OpenAI-specific capabilities. Use `ITextGeneration` in your application code when possible to remain provider-agnostic.

---

## Usage in a handler

```csharp
public sealed class GenerateOrderSummaryCommandHandler(ITextGeneration ai)
    : IUnitResultRequestHandler<GenerateOrderSummaryCommand, string>
{
    public async Task<IUnitResult<string>> Handle(
        GenerateOrderSummaryCommand request,
        CancellationToken cancellationToken)
    {
        var aiRequest = new AIServiceRequest
        {
            Prompt = $"Summarise this order: {request.OrderDescription}"
        };

        var response = await ai.GenerateTextAsync(aiRequest, cancellationToken);

        return UnitResult.FromResult(response.Text, UnitAction.Get);
    }
}
```

---

## Connection verification

Call `VerifyConnectionAsync` at startup or as a health check:

```csharp
var result = await textGeneration.VerifyConnectionAsync(cancellationToken);
if (!result.IsConnected)
    logger.LogWarning("AI service is not reachable: {Reason}", result.FailureReason);
```

---

## Uncertainty note

> The AI packages are present in the repository but do not yet have detailed integration tests. The public API shape described above is accurate as of the current codebase, but the exact provider behaviour (retry policies, streaming support, etc.) has not been fully verified. Review the source in `src/IDFCR.AI/` and confirm with the maintainer before relying on these packages in production.

---

## Further reading

- [Package map](package-map.md) — all AI packages and their dependencies.
- [Getting started](getting-started.md) — general pattern for adding optional packages.
