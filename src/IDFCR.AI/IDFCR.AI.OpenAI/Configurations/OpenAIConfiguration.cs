using IDFCR.AI.Http.Configurations;

namespace IDFCR.AI.OpenAI.Configurations;

/// <summary>
/// Configuration for the OpenAI transport.
/// </summary>
public sealed record OpenAIConfiguration : HttpAIServiceConfiguration
{
    /// <summary>
    /// Gets or sets the API key used for authentication.
    /// </summary>
    public string ApiKey { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the default model used when a request does not specify one.
    /// </summary>
    public string Model { get; init; } = "gpt-4.1-mini";

    /// <summary>
    /// Gets or sets the optional OpenAI organization identifier.
    /// </summary>
    public string? Organization { get; init; }

    /// <summary>
    /// Gets or sets the optional OpenAI project identifier.
    /// </summary>
    public string? Project { get; init; }

    /// <summary>
    /// Creates an <see cref="OpenAIConfiguration"/> with OpenAI-specific defaults.
    /// </summary>
    /// <param name="apiKey">The API key used to authenticate with OpenAI.</param>
    /// <returns>A configuration pre-populated with the standard OpenAI endpoint settings.</returns>
    public static OpenAIConfiguration Create(string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);

        return new OpenAIConfiguration
        {
            ServiceName = "OpenAI",
            ApiKey = apiKey,
            BaseAddress = new Uri("https://api.openai.com/"),
            VerificationPath = "v1/models",
            VerificationMethod = HttpMethod.Get.Method,
            Timeout = TimeSpan.FromSeconds(60)
        };
    }
}
