namespace IDFCR.Abstractions.Persistence.StorageQueues;

/// <summary>
/// Represents a consumer that can acknowledge messages from a queue. The AcknowledgeMessageAsync method takes a message ID and acknowledges the message asynchronously, allowing for proper message processing and removal from the queue. This interface can be implemented by classes that interact with different queueing systems, providing a consistent way to acknowledge messages regardless of the underlying implementation.
/// </summary>
public interface IQueueConsumer<TPullResponse, TResult, TMessageItem, TError, TBody>
    where TPullResponse : IQueuePullResponse<TResult, TMessageItem, TError, TBody>
    where TMessageItem : IQueueMessageItem<TBody>
    where TError : IApiError
    where TResult : IQueuePullResult<TMessageItem, TBody>
{
    /// <summary>
    /// Pulls messages from the queue asynchronously with the specified visibility timeout and batch size. The method returns an enumerable of TPullResponse objects, allowing for processing of the pulled messages. The visibility timeout parameter specifies how long the messages will remain invisible to other consumers after being pulled, while the batch size parameter determines how many messages to pull in a single operation.
    /// </summary>
    /// <param name="VisibilityTimeout">The duration, in seconds, that the pulled messages will remain visible to other consumers.</param>
    /// <param name="BatchSize">The number of messages to pull in a single operation.</param>
    /// <returns>An enumerable of TPullResponse objects representing the pulled messages.</returns>
    Task<IEnumerable<TMessageItem>> PullMessagesAsync(
        int VisibilityTimeout, 
        int BatchSize,
        CancellationToken cancellationToken);
    /// <summary>
    /// Acknowledges a message with the specified message ID from the queue asynchronously. The method allows for proper message processing and removal from the queue, ensuring that messages are not processed multiple times. The cancellationToken parameter can be used to cancel the operation if needed.
    /// </summary>
    /// <param name="messageId">The ID of the message to acknowledge.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AcknowledgeMessageAsync(string messageId, CancellationToken cancellationToken);
}
