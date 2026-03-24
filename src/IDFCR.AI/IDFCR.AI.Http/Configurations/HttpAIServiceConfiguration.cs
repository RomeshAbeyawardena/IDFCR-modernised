using IDFCR.AI.Abstractions;

namespace IDFCR.AI.Http.Configurations;

public record HttpAIServiceConfiguration : IAIServiceConfiguration
{
    public string ServiceName { get; init; } = nameof(HttpAIServiceConfiguration);

    public TimeSpan? Timeout { get; init; }

    public required Uri BaseAddress { get; init; }

    public string VerificationPath { get; init; } = "/";

    public string VerificationMethod { get; init; } = HttpMethod.Get.Method;

    public string? VerificationContent { get; init; }

    public string? VerificationContentType { get; init; }

    public IDictionary<string, string> DefaultHeaders { get; init; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}
