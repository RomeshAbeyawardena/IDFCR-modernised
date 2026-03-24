using System.Text.Json;
using IDFCR.AI.Abstractions;
using IDFCR.AI.OpenAI.Configurations;
using IDFCR.AI.OpenAI.Exceptions;
using IDFCR.AI.OpenAI.Models;

namespace IDFCR.AI.OpenAI;

public sealed class OpenAIService(IAIService aiService) : IOpenAIService
{
    private const string JsonContentType = "application/json";

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public Task<VerifiedConnectionResult> VerifyConnection(OpenAIConfiguration configuration, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        return aiService.VerifyConnection(configuration, cancellationToken);
    }

    public async Task<OpenAITextResponse> GenerateTextAsync(OpenAIConfiguration configuration, OpenAITextRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Prompt);

        var payload = new Dictionary<string, object?>
        {
            ["model"] = string.IsNullOrWhiteSpace(request.Model) ? configuration.Model : request.Model,
            ["input"] = request.Prompt
        };

        if (!string.IsNullOrWhiteSpace(request.Instructions))
        {
            payload["instructions"] = request.Instructions;
        }

        if (request.Temperature is { } temperature)
        {
            payload["temperature"] = temperature;
        }

        var response = await aiService.SendAsync(configuration, new AIServiceRequest
        {
            Method = HttpMethod.Post.Method,
            RelativePath = "v1/responses",
            Content = JsonSerializer.Serialize(payload, SerializerOptions),
            ContentType = JsonContentType,
            Headers = new Dictionary<string, string>(request.Headers, StringComparer.OrdinalIgnoreCase)
        }, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new OpenAIHttpException(
                $"OpenAI request failed with status code {(int)response.StatusCode}.",
                response.StatusCode,
                response.Content);
        }

        var rawContent = response.Content ?? string.Empty;

        if (string.IsNullOrWhiteSpace(rawContent))
        {
            return new OpenAITextResponse(null, null, null, rawContent, response.StatusCode);
        }

        using var document = JsonDocument.Parse(rawContent);
        var root = document.RootElement;

        return new OpenAITextResponse(
            GetOptionalString(root, "id"),
            GetOptionalString(root, "status"),
            ExtractOutputText(root),
            rawContent,
            response.StatusCode);
    }

    private static string? ExtractOutputText(JsonElement root)
    {
        if (TryGetOptionalString(root, "output_text", out var outputText))
        {
            return outputText;
        }

        if (!root.TryGetProperty("output", out var output) || output.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        foreach (var item in output.EnumerateArray())
        {
            if (!item.TryGetProperty("content", out var content) || content.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (var contentItem in content.EnumerateArray())
            {
                if (TryGetOptionalString(contentItem, "text", out var text))
                {
                    return text;
                }
            }
        }

        return null;
    }

    private static string? GetOptionalString(JsonElement element, string propertyName)
    {
        return TryGetOptionalString(element, propertyName, out var value) ? value : null;
    }

    private static bool TryGetOptionalString(JsonElement element, string propertyName, out string? value)
    {
        value = null;

        if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind == JsonValueKind.Null)
        {
            return false;
        }

        value = property.GetString();
        return value is not null;
    }
}
