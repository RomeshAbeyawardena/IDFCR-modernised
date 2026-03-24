using System.Net;

namespace IDFCR.AI.Abstractions;

/// <summary>
/// Represents the raw response returned by an <see cref="IAIService"/>.
/// </summary>
/// <param name="StatusCode">The status code returned by the service.</param>
/// <param name="Content">The response body, if any.</param>
/// <param name="Headers">The response headers, including content headers.</param>
public sealed record AIServiceResponse(HttpStatusCode StatusCode, string? Content, IReadOnlyDictionary<string, IReadOnlyCollection<string>> Headers)
{
    /// <summary>
    /// Gets a value indicating whether the status code is in the 2xx range.
    /// </summary>
    public bool IsSuccessStatusCode => (int)StatusCode is >= 200 and <= 299;
}
