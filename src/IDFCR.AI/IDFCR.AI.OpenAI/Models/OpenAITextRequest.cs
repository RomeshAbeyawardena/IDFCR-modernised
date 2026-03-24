namespace IDFCR.AI.OpenAI.Models;

/// <summary>
/// Represents a text-generation request for OpenAI's Responses API.
/// </summary>
public sealed record OpenAITextRequest
{
    /// <summary>
    /// Gets or sets the prompt text sent as the request input.
    /// </summary>
    public required string Prompt { get; init; }

    /// <summary>
    /// Gets or sets optional instructions forwarded to the API.
    /// </summary>
    public string? Instructions { get; init; }

    /// <summary>
    /// Gets or sets the model to use for this request.
    /// </summary>
    public string? Model { get; init; }

    /// <summary>
    /// Gets or sets the sampling temperature.
    /// </summary>
    public double? Temperature { get; init; }

    /// <summary>
    /// Gets or sets additional headers to include on the outgoing request.
    /// </summary>
    public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}
