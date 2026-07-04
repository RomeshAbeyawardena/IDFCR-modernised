using System.Text.Json.Serialization;

namespace IDFCR.Persistence.CloudFlare.Queues;

/// <summary>
/// Represents a request to acknowledge messages in a Cloudflare queue. The class contains a list of CloudflareRetryItem objects, each representing a message to be acknowledged with its corresponding lease ID and optional delay in seconds for retrying the message. The class is used to construct the payload for the acknowledgment request sent to the Cloudflare API.
/// </summary>
public record CloudflareRetryItem
{
    /// <summary>
    /// Gets or sets the lease ID of the message to be acknowledged. The lease ID is a unique identifier for the message in the Cloudflare queue, and it is used to identify which message should be acknowledged and removed from the queue.
    /// </summary>
    [JsonPropertyName("lease_id")]
    public string? LeaseId { get; init; }

    /// <summary>
    /// Gets or sets the delay in seconds before the message is retried. This property can be used to explicitly push a message back into the queue for a retry after a specified delay. If not set, the message will be retried immediately.
    /// </summary>
    [JsonPropertyName("delay_seconds")]
    public int DelaySeconds { get; set; } // Can be used to explicitly push a message back into the queue for a retry
}
