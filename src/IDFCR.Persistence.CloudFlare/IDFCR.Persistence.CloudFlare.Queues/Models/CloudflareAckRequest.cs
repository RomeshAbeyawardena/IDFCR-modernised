using System.Text.Json.Serialization;

namespace IDFCR.Persistence.CloudFlare.Queues.Models;

/// <summary>
/// Represents a request to acknowledge messages in a Cloudflare queue. The class contains two properties: Acks, which is a list of CloudFlareAckItem objects representing the messages to be acknowledged, and Retries, which is a list of CloudFlareRetryItem objects representing the messages to be retried. The Acks property is used to acknowledge successful message processing, while the Retries property can be used to explicitly push messages back into the queue for retrying. The class is serialized to JSON when sending the acknowledgment request to the Cloudflare API.
/// </summary>
public record CloudFlareAckRequest
{
    /// <summary>
    /// Gets or sets the list of messages to be acknowledged. Each item in the list is a CloudFlareAckItem object that contains the lease ID of the message to be acknowledged. The Acks property is used to acknowledge successful message processing and remove the messages from the queue.
    /// </summary>
    [JsonPropertyName("acks")]
    public List<CloudFlareAckItem> Acks { get; init; } = [];

    /// <summary>
    /// Gets or sets the list of messages to be retried. Each item in the list is a CloudFlareRetryItem object that contains the lease ID of the message to be retried and the delay in seconds before the message should be retried. The Retries property can be used to explicitly push messages back into the queue for retrying, allowing for controlled retries of failed message processing. If there are no messages to retry, this property can be left empty.
    /// </summary>
    [JsonPropertyName("retries")]
    public List<CloudFlareRetryItem> Retries { get; init; } = []; // Left empty if strictly acknowledging success
}
