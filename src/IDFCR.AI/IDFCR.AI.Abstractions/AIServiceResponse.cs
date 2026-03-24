using System.Net;

namespace IDFCR.AI.Abstractions;

public sealed record AIServiceResponse(HttpStatusCode StatusCode, string? Content, IReadOnlyDictionary<string, IReadOnlyCollection<string>> Headers)
{
    public bool IsSuccessStatusCode => (int)StatusCode is >= 200 and <= 299;
}
