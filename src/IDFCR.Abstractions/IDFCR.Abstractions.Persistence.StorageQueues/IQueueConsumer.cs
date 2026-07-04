namespace IDFCR.Abstractions.Persistence.StorageQueues;

/// <summary>
/// Represents a consumer that can acknowledge messages from a queue. The AcknowledgeMessageAsync method takes a message ID and acknowledges the message asynchronously, allowing for proper message processing and removal from the queue. This interface can be implemented by classes that interact with different queueing systems, providing a consistent way to acknowledge messages regardless of the underlying implementation.
/// </summary>
public interface IQueueConsumer
{
    /// <summary>
    /// Acknowledges a message with the specified message ID from the queue asynchronously. The method allows for proper message processing and removal from the queue, ensuring that messages are not processed multiple times. The cancellationToken parameter can be used to cancel the operation if needed.
    /// </summary>
    /// <param name="messageId">The ID of the message to acknowledge.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AcknowledgeMessageAsync(string messageId, CancellationToken cancellationToken);
}
