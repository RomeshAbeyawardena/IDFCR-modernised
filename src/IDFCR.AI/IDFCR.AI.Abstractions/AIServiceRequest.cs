namespace IDFCR.AI.Abstractions;

/// <summary>
/// Represents a request sent through an <see cref="IAIService"/>.
/// </summary>
public sealed record AIServiceRequest
{
    /// <summary>
    /// Gets or sets the HTTP method to use for the request.
    /// </summary>
    public required string Method { get; init; }

    /// <summary>
    /// Gets or sets the relative path to combine with the configured base address.
    /// </summary>
    public string RelativePath { get; init; } = "/";

    /// <summary>
    /// Gets or sets the request body payload.
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// Gets or sets the content type associated with <see cref="Content"/>.
    /// </summary>
    public string? ContentType { get; init; }

    /// <summary>
    /// Gets or sets headers applied to the outgoing request.
    /// </summary>
    public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}
