using System.Text.Json.Serialization;

namespace IDFCR.Persistence.CloudFlare.Queues.Models;

/// <summary>
/// Represents an item to be acknowledged in a Cloudflare queue. The class contains a single property, LeaseId, which is the unique identifier of the message to be acknowledged. The LeaseId is used by the Cloudflare API to identify and remove the message from the queue upon successful acknowledgment. This class is serialized to JSON when sending the acknowledgment request to the Cloudflare API.
/// </summary>
public record CloudflareAckItem
{
    /// <summary>
    /// Gets or sets the lease ID of the message to be acknowledged. The LeaseId is a unique identifier assigned to the message when it is pulled from the queue, and it is required by the Cloudflare API to acknowledge and remove the message from the queue. This property is serialized to JSON with the name "lease_id" when sending the acknowledgment request to the Cloudflare API.
    /// </summary>
    [JsonPropertyName("lease_id")]
    public string? LeaseId { get; init; }
}
