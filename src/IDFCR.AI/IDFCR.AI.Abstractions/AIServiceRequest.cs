namespace IDFCR.AI.Abstractions;

public sealed record AIServiceRequest
{
    public required string Method { get; init; }

    public string RelativePath { get; init; } = "/";

    public string? Content { get; init; }

    public string? ContentType { get; init; }

    public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}
