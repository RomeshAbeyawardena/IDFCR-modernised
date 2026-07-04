using IDFCR.Abstractions.Metadata;

namespace IDFCR.Abstractions.Persistence.StorageQueues;

/// <summary>
/// Represents a message item in a queue, providing access to the message's body and metadata. The interface extends the <see cref="IIdentifiable"/> interface, allowing for identification of the message item. It includes properties for the timestamp of when the message was created or pulled, the number of attempts made to process the message (if applicable), and the body of the message itself. This interface can be implemented by classes that represent messages in different queueing systems, providing a consistent way to access message information regardless of the underlying implementation.
/// </summary>
/// <typeparam name="TBody">The type of the message body.</typeparam>
public interface IQueueMessageItem<TBody> : IIdentifiable
{
    /// <summary>
    /// Gets the timestamp of when the message was created or pulled. This property provides information about the timing of the message, which can be useful for tracking and processing purposes.
    /// </summary>
    long Timestamp { get; }
    /// <summary>
    /// Gets the number of attempts made to process the message, if applicable. This property can be used to track how many times a message has been retried or processed, allowing for handling of messages that may require multiple attempts for successful processing.
    /// </summary>
    int? Attempts { get; }
    /// <summary>
    /// Gets the body of the message. This property provides access to the actual content of the message, allowing for processing and handling of the message data.
    /// </summary>
    TBody Body { get; }
}
