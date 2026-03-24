using System.Text.Json;
using IDFCR.AI.Abstractions;
using IDFCR.AI.OpenAI.Configurations;
using IDFCR.AI.OpenAI.Exceptions;
using IDFCR.AI.OpenAI.Models;

namespace IDFCR.AI.OpenAI;

/// <summary>
/// OpenAI-specific wrapper around <see cref="IAIService"/>.
/// </summary>
public sealed class OpenAIService(IAIService aiService) : IOpenAIService
{
    private const string JsonContentType = "application/json";

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    /// <inheritdoc />
    public Task<VerifiedConnectionResult> VerifyConnection(OpenAIConfiguration configuration, CancellationToken cancellationToken)
        => VerifyConnectionCoreAsync(configuration, cancellationToken);

    /// <inheritdoc />
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
            Headers = BuildHeaders(configuration, request.Headers)
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
                if (TryGetOptionalString(contentItem, "type", out var contentType)
                    && string.Equals(contentType, "output_text", StringComparison.Ordinal)
                    && TryGetOptionalString(contentItem, "text", out var text))
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

    private async Task<VerifiedConnectionResult> VerifyConnectionCoreAsync(OpenAIConfiguration configuration, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        try
        {
            var response = await aiService.SendAsync(configuration, new AIServiceRequest
            {
                Method = configuration.VerificationMethod,
                RelativePath = configuration.VerificationPath,
                Content = configuration.VerificationContent,
                ContentType = configuration.VerificationContentType,
                Headers = BuildHeaders(configuration)
            }, cancellationToken);

            return response.IsSuccessStatusCode
                ? VerifiedConnectionResult.Success(response.StatusCode)
                : VerifiedConnectionResult.Failure("The AI service responded, but the connection was not accepted.", response.StatusCode);
        }
        catch (Exception exception)
        {
            return VerifiedConnectionResult.Failure(exception.Message);
        }
    }

    private static Dictionary<string, string> BuildHeaders(OpenAIConfiguration configuration, IDictionary<string, string>? requestHeaders = null)
    {
        var headers = new Dictionary<string, string>(configuration.DefaultHeaders, StringComparer.OrdinalIgnoreCase);

        headers["Authorization"] = $"Bearer {configuration.ApiKey}";

        if (!string.IsNullOrWhiteSpace(configuration.Organization))
        {
            headers["OpenAI-Organization"] = configuration.Organization;
        }

        if (!string.IsNullOrWhiteSpace(configuration.Project))
        {
            headers["OpenAI-Project"] = configuration.Project;
        }

        if (requestHeaders is null)
        {
            return headers;
        }

        foreach (var (key, value) in requestHeaders)
        {
            headers[key] = value;
        }

        return headers;
    }
}
