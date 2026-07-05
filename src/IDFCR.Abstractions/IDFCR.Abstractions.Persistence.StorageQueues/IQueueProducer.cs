using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Persistence.StorageQueues;

/// <summary>
/// Represents a producer that can send messages to a queue. The SendMessageAsync method takes a payload of type T and sends it to the queue asynchronously, returning a boolean indicating whether the message was sent successfully. This interface can be implemented by classes that interact with different queueing systems, allowing for a consistent way to send messages regardless of the underlying implementation.
/// </summary>
public interface IQueueProducer
{
    /// <summary>
    /// Sends a message with the specified payload to the queue asynchronously. The method returns a boolean indicating whether the message was sent successfully. The payload can be of any type T, allowing for flexibility in the types of messages that can be sent to the queue.
    /// </summary>
    /// <typeparam name="T">The type of the message payload.</typeparam>
    /// <param name="payload">The message payload to send.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the message was sent successfully.</returns>
    Task<IUnitResult> SendMessageAsync<T>(T payload, CancellationToken cancellationToken);
}