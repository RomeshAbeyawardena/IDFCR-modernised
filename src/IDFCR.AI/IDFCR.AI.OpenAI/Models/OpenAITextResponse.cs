using System.Net;

namespace IDFCR.AI.OpenAI.Models;

public sealed record OpenAITextResponse(
    string? Id,
    string? Status,
    string? OutputText,
    string RawContent,
    HttpStatusCode StatusCode);
