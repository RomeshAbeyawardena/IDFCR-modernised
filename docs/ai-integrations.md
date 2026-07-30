# AI integrations

IDFCR provides a thin set of AI abstractions and provider implementations, following the same opt-in, contract-first approach as the rest of the framework.

**Packages:** `IDFCR.AI.Abstractions`, `IDFCR.AI.Http`, `IDFCR.AI.OpenAI`

---

## Core abstraction

### IAIService

`IAIService` (in `IDFCR.AI.Abstractions`) is the low-level transport contract for AI providers. It exposes two generic methods that accept a provider-specific configuration:

```csharp
public interface IAIService
{
    // Checks that the configured AI provider is reachable and accepting requests.
    Task<VerifiedConnectionResult> VerifyConnection<TConfiguration>(
        TConfiguration configuration, CancellationToken cancellationToken)
        where TConfiguration : IAIServiceConfiguration;

    // Sends a raw request to the provider and returns the HTTP-level response.
    Task<AIServiceResponse> SendAsync<TConfiguration>(
        TConfiguration configuration,
        AIServiceRequest request,
        CancellationToken cancellationToken)
        where TConfiguration : IAIServiceConfiguration;
}
```

`AIServiceRequest` is a transport-level record:

```csharp
var request = new AIServiceRequest
{
    Method  = "POST",
    RelativePath = "v1/completions",
    Content = """{"model":"gpt-4.1-mini","input":"Hello"}""",
    ContentType = "application/json"
};
```

`AIServiceResponse` returns `StatusCode`, `Content`, `Headers`, and `IsSuccessStatusCode`.

---

## HTTP provider

`IDFCR.AI.Http` provides `HttpAIService`, a generic HTTP-backed implementation that routes to any AI endpoint.

### Registration

```csharp
using IDFCR.AI.Http.Extensions;

services.AddHttpAIService();
```

This registers `IAIService` as `HttpAIService` via `HttpClient`.

---

## OpenAI provider

`IDFCR.AI.OpenAI` provides `OpenAIService` and the higher-level `IOpenAIService` contract, which exposes OpenAI-specific operations built on top of `IAIService`.

### Registration

```csharp
using IDFCR.AI.OpenAI.Extensions;

services.AddOpenAI();
```

`AddOpenAI()` calls `AddHttpAIService()` internally and registers `IOpenAIService`.

### IOpenAIService

```csharp
public interface IOpenAIService
{
    Task<VerifiedConnectionResult> VerifyConnection(
        OpenAIConfiguration configuration,
        CancellationToken cancellationToken);

    Task<OpenAITextResponse> GenerateTextAsync(
        OpenAIConfiguration configuration,
        OpenAITextRequest request,
        CancellationToken cancellationToken);
}
```

`OpenAITextRequest` carries the prompt and generation parameters:

```csharp
var request = new OpenAITextRequest
{
    Prompt       = "Summarise this order: ...",
    Model        = "gpt-4o",      // optional; defaults to gpt-4.1-mini
    Instructions = "Be concise.", // optional system instructions
    Temperature  = 0.7            // optional
};
```

`OpenAITextResponse` returns `OutputText`, `Id`, `Status`, `RawContent`, and `StatusCode`.

`OpenAIConfiguration` holds `ApiKey`, `Model`, `Organization`, `Project`, and the base address. The static factory method provides sensible defaults:

```csharp
var config = OpenAIConfiguration.Create(apiKey: "sk-...");
```

### Usage in a handler

```csharp
public sealed class GenerateOrderSummaryCommandHandler(IOpenAIService ai)
    : IUnitResultRequestHandler<GenerateOrderSummaryCommand, string>
{
    public async Task<IUnitResult<string>> Handle(
        GenerateOrderSummaryCommand request,
        CancellationToken cancellationToken)
    {
        var config = OpenAIConfiguration.Create(request.ApiKey);
        var aiRequest = new OpenAITextRequest
        {
            Prompt = $"Summarise this order: {request.OrderDescription}"
        };

        var response = await ai.GenerateTextAsync(config, aiRequest, cancellationToken);

        return UnitResult.FromResult(response.OutputText ?? string.Empty, UnitAction.Get);
    }
}
```

---

## Connection verification

```csharp
var config = OpenAIConfiguration.Create(apiKey);
var result = await openAIService.VerifyConnection(config, cancellationToken);
if (!result.IsConnected)
    logger.LogWarning("AI service unreachable: {Reason}", result.FailureReason);
```

---

## Uncertainty note

> The AI packages are present in the repository but do not yet have detailed integration tests. The `ITextGeneration` interface exists in the source but is `internal` and intentionally unexposed — use `IOpenAIService` for OpenAI-specific work and `IAIService` for custom provider integrations. Confirm provider behaviour (retry policies, streaming support, etc.) with the maintainer before relying on these packages in production.

---

## Further reading

- [Package map](package-map.md) — all AI packages and their dependencies.
- [Getting started](getting-started.md) — general pattern for adding optional packages.
