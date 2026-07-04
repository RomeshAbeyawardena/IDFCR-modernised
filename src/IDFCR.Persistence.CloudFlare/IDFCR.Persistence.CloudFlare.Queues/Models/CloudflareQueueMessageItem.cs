using IDFCR.Abstractions.Persistence.StorageQueues;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IDFCR.Persistence.CloudFlare.Queues.Models;

/// <summary>
/// Represents a message item retrieved from a queue. The QueueMessageItem class contains properties for the unique message ID, lease ID, timestamp, number of delivery attempts, and the message body. The message body is of type JsonElement, allowing it to capture generic JSON data structures dynamically. This class implements the IQueueMessageItem interface, providing a consistent way to represent messages in different queueing systems.
/// </summary>
public class CloudflareQueueMessageItem : IQueueMessageItem<JsonElement>
{
    /// <summary>
    /// Gets or sets the unique message ID associated with the queue message. This property is of type object and can hold any value that uniquely identifies the message within the queue. The unique message ID is used for acknowledgment purposes, allowing the system to track and manage the processing of messages. The value of this property can be used to acknowledge the receipt and processing of the message in the queue system.
    /// </summary>
    [JsonPropertyName("id")]
    public object? Id { get; init; } // The Unique Message ID used to ACK the item later

    /// <summary>
    /// Gets or sets the lease ID associated with the queue message. This property is a string that represents the lease ID required for acknowledgment mappings. The lease ID is used to manage the acknowledgment of messages in the queue, ensuring that messages are processed and acknowledged correctly. The value of this property can be used to track and manage message processing within the queue system.
    /// </summary>
    [JsonPropertyName("lease_id")]
    public string? LeaseId { get; init; } // The lease ID required for acknowledgment mappings

    /// <summary>
    /// Gets or sets the timestamp of when the queue message was sent. This property is a long integer representing the epoch millisecond marker, which indicates the exact time the message was sent to the queue. The value of this property can be used for tracking message timing, ordering, and processing within the queue system.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; init; } // Epoch millisecond marker when sent

    /// <summary>
    /// Gets or sets the number of delivery attempts for the queue message. This property is an integer that tracks how many times the message has been attempted to be delivered. It is nullable, allowing for cases where the number of attempts may not be known or applicable. The value of this property can be used to implement retry logic or to monitor the delivery status of messages in the queue.
    /// </summary>
    [JsonPropertyName("attempts")]
    public int? Attempts { get; init; } // Track delivery attempt loops

    /// <summary>
    /// Gets or sets the body of the queue message. This property is of type JsonElement, allowing it to capture generic JSON data structures dynamically. The value of this property can be any valid JSON object, array, string, number, boolean, or null, providing flexibility in the types of messages that can be sent and received through the queue.
    /// </summary>
    [JsonPropertyName("body")]
    public JsonElement Body { get; init; } // Captures generic JSON data structures dynamically
}
