namespace IDFCR.AI.OpenAI.Models;

public sealed record OpenAITextRequest
{
    public required string Prompt { get; init; }

    public string? Instructions { get; init; }

    public string? Model { get; init; }

    public double? Temperature { get; init; }

    public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}
