using System.Net;

namespace IDFCR.AI.OpenAI.Models;

/// <summary>
/// Represents a parsed OpenAI text-generation response.
/// </summary>
/// <param name="Id">The response identifier returned by OpenAI, if present.</param>
/// <param name="Status">The provider status string, if present.</param>
/// <param name="OutputText">The extracted response text, if it could be located.</param>
/// <param name="RawContent">The raw JSON payload returned by the API.</param>
/// <param name="StatusCode">The HTTP status code returned by the API.</param>
public sealed record OpenAITextResponse(
    string? Id,
    string? Status,
    string? OutputText,
    string RawContent,
    HttpStatusCode StatusCode);
