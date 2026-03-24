using IDFCR.AI.Http.Configurations;

namespace IDFCR.AI.OpenAI.Configurations;

public sealed record OpenAIConfiguration : HttpAIServiceConfiguration
{
    public string ApiKey { get; init; } = string.Empty;

    public string Model { get; init; } = "gpt-4.1-mini";

    public string? Organization { get; init; }

    public string? Project { get; init; }

    public static OpenAIConfiguration Create(string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);

        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Authorization"] = $"Bearer {apiKey}"
        };

        return new OpenAIConfiguration
        {
            ServiceName = "OpenAI",
            ApiKey = apiKey,
            BaseAddress = new Uri("https://api.openai.com/"),
            VerificationPath = "v1/models",
            VerificationMethod = HttpMethod.Get.Method,
            Timeout = TimeSpan.FromSeconds(60),
            DefaultHeaders = headers
        };
    }
}
