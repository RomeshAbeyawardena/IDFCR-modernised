using IDFCR.Abstractions.Metadata;

namespace IDFCR.Abstractions.Interceptors.Handlers;

/// <summary>
/// Represents an entity that can be used in an outbox pattern for reliable message delivery and tracking of message status. This interface defines the structure of an outbox entity, which includes properties for storing the data of the message, as well as timestamps for when the message was completed, failed, or acknowledged. By implementing this interface, developers can create custom logic for processing outbox messages within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status. The outbox entity can be designed to be stored in a database or other persistent storage mechanism, allowing for the reliable tracking and management of messages as they are processed and delivered within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
/// </summary>
public interface IOutboxEntity : IAuditCreatedTimestamp, IAuditModifiedTimestamp
{
    /// <summary>
    /// Gets or sets the unique identifier for the outbox message, which can be used to track and manage the message within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status. This property allows for the identification and correlation of outbox messages, enabling developers to implement logic for handling specific messages based on their unique identifiers, such as retrying failed messages, acknowledging successful deliveries, or performing other actions based on the status of the outbox message within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    object Id { get; set; }
    /// <summary>
    /// Gets or sets the data associated with the outbox message, which can include the payload or content of the message being processed. This property allows for the storage of relevant information related to the outbox message, such as the message body, metadata, or any other data that is necessary for processing and tracking the status of the message within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    string? Data { get; set; }
    /// <summary>
    /// Gets or sets the timestamp indicating when the outbox message was completed, failed, or acknowledged. This property allows for the tracking of the status of the outbox message, providing information about when the message was processed successfully (completed), when it encountered an error (failed), or when it was acknowledged by the recipient. By utilizing these timestamps, developers can implement logic for handling outbox messages based on their status and ensure reliable message delivery and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    DateTimeOffset? CompletedTimestampUtc { get; set; }
    /// <summary>
    /// Gets or sets the timestamp indicating when the outbox message failed to be processed. This property allows for the tracking of errors or issues that may occur during the processing of the outbox message, providing information about when the failure occurred. By utilizing this timestamp, developers can implement logic for handling failed outbox messages, such as retrying the message processing, logging the failure for further analysis, or taking appropriate actions based on the failure status within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    DateTimeOffset? FailedTimestampUtc { get; set; }
    /// <summary>
    /// Gets or sets the timestamp indicating when the outbox message was acknowledged by the recipient. This property allows for the tracking of when the message was successfully received and acknowledged by the intended recipient, providing information about the status of the message delivery. By utilizing this timestamp, developers can implement logic for handling acknowledged outbox messages, such as marking the message as completed, removing it from the outbox queue, or taking appropriate actions based on the acknowledgment status within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    DateTimeOffset? AcknowledgedTimestampUtc { get; set; }
}

/// <summary>
/// Represents an interceptor for handling outbox entities, allowing for the processing of outbox messages and the tracking of their status. This interface defines the structure of an outbox entity, which includes properties for storing the data of the message, as well as timestamps for when the message was completed, failed, or acknowledged. By implementing this interface, developers can create custom logic for processing outbox messages within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status. The interceptor can be designed to be applied at various stages of the entity lifecycle, allowing for flexible handling of outbox messages based on specific requirements and use cases related to message processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
/// </summary>
/// <typeparam name="TKey"></typeparam>
public interface IOutboxEntity<TKey> : IIdentifiable<TKey>, IOutboxEntity
    where TKey : struct
{
    
}
