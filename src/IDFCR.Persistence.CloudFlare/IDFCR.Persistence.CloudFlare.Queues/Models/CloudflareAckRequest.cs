using System.Text.Json.Serialization;

namespace IDFCR.Persistence.CloudFlare.Queues.Models;

/// <summary>
/// Represents a request to acknowledge messages in a Cloudflare queue. The class contains two properties: Acks, which is a list of CloudflareAckItem objects representing the messages to be acknowledged, and Retries, which is a list of CloudflareRetryItem objects representing the messages to be retried. The Acks property is used to acknowledge successful message processing, while the Retries property can be used to explicitly push messages back into the queue for retrying. The class is serialized to JSON when sending the acknowledgment request to the Cloudflare API.
/// </summary>
public record CloudflareAckRequest
{
    /// <summary>
    /// Gets or sets the list of messages to be acknowledged. Each item in the list is a CloudflareAckItem object that contains the lease ID of the message to be acknowledged. The Acks property is used to acknowledge successful message processing and remove the messages from the queue.
    /// </summary>
    [JsonPropertyName("acks")]
    public List<CloudflareAckItem> Acks { get; init; } = [];

    /// <summary>
    /// Gets or sets the list of messages to be retried. Each item in the list is a CloudflareRetryItem object that contains the lease ID of the message to be retried and the delay in seconds before the message should be retried. The Retries property can be used to explicitly push messages back into the queue for retrying, allowing for controlled retries of failed message processing. If there are no messages to retry, this property can be left empty.
    /// </summary>
    [JsonPropertyName("retries")]
    public List<CloudflareRetryItem> Retries { get; init; } = []; // Left empty if strictly acknowledging success
}
