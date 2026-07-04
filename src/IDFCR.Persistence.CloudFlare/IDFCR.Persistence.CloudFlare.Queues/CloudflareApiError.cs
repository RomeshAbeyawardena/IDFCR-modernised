using IDFCR.Abstractions.Persistence.StorageQueues;
using System.Text.Json.Serialization;

namespace IDFCR.Persistence.CloudFlare.Queues;

/// <summary>
/// Represents an error returned by the Cloudflare API. The CloudflareApiError record contains two properties: Code, which is an integer representing the error code, and Message, which is a string providing a description of the error. This record implements the IApiError interface, allowing it to be used in contexts where API errors need to be handled or logged. The properties are initialized using the init accessor, making them immutable after object creation.
/// </summary>
public record CloudflareApiError : IApiError
{
    /// <summary>
    /// Gets the error code returned by the Cloudflare API. This property is an integer that represents the specific error encountered during the API request. The value of this property is set during object initialization and cannot be modified afterward.
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; init; }

    /// <summary>
    /// Gets the error message returned by the Cloudflare API. This property is a string that provides a description of the error encountered during the API request. The value of this property is set during object initialization and cannot be modified afterward.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }
}