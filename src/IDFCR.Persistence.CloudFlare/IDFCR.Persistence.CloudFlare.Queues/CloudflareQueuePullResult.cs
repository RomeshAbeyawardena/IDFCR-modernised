using IDFCR.Abstractions.Persistence.StorageQueues;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IDFCR.Persistence.CloudFlare.Queues;

/// <summary>
/// Represents the result of pulling messages from a Cloudflare queue. The QueuePullResult class contains properties for the count of messages in the backlog and a list of QueueMessageItem objects representing the messages that were pulled from the queue. This class implements the IQueuePullResult interface, allowing for consistent handling of queue pull results across different implementations.
/// </summary>
public class CloudflareQueuePullResult 
    : IQueuePullResult<CloudflareQueueMessageItem, JsonElement>
{
    IEnumerable<CloudflareQueueMessageItem> IQueuePullResult<CloudflareQueueMessageItem, JsonElement>.Messages => Messages;

    /// <summary>
    /// Gets or sets the count of messages in the backlog. This property provides information about the number of messages that are currently waiting to be processed in the queue, allowing for monitoring and management of the queue's state.
    /// </summary>
    [JsonPropertyName("message_backlog_count")]
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the list of QueueMessageItem objects representing the messages that were pulled from the queue. This property provides access to the actual messages that were retrieved, allowing for processing and handling of the messages as needed.
    /// </summary>
    [JsonPropertyName("messages")]
    public List<CloudflareQueueMessageItem> Messages { get; set; } = new();
}
