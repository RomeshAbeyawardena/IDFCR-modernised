using IDFCR.AI.Abstractions;

namespace IDFCR.AI.Http.Configurations;

/// <summary>
/// Configuration used by <see cref="HttpAIService"/> to send requests over <see cref="HttpClient"/>.
/// </summary>
public record HttpAIServiceConfiguration : IAIServiceConfiguration
{
    /// <summary>
    /// Gets or sets the logical name of the service.
    /// </summary>
    public string ServiceName { get; init; } = nameof(HttpAIServiceConfiguration);

    /// <summary>
    /// Gets or sets the request timeout applied by the HTTP AI service.
    /// </summary>
    public TimeSpan? Timeout { get; init; }

    /// <summary>
    /// Gets or sets the base address used to build outgoing request URIs.
    /// </summary>
    public required Uri BaseAddress { get; init; }

    /// <summary>
    /// Gets or sets the relative path used when verifying the connection.
    /// </summary>
    public string VerificationPath { get; init; } = "/";

    /// <summary>
    /// Gets or sets the HTTP method used when verifying the connection.
    /// </summary>
    public string VerificationMethod { get; init; } = HttpMethod.Get.Method;

    /// <summary>
    /// Gets or sets the optional verification request body.
    /// </summary>
    public string? VerificationContent { get; init; }

    /// <summary>
    /// Gets or sets the optional content type for the verification request body.
    /// </summary>
    public string? VerificationContentType { get; init; }

    /// <summary>
    /// Gets or sets headers applied to every outgoing request.
    /// </summary>
    public IDictionary<string, string> DefaultHeaders { get; init; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}
