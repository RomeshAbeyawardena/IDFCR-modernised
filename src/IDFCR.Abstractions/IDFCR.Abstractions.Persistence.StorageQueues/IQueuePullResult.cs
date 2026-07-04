namespace IDFCR.Abstractions.Persistence.StorageQueues;

/// <summary>
/// Represents the result of pulling messages from a queue. The interface provides properties to access the count of messages pulled and an enumerable collection of message items. Each message item is of type TMessageItem, which implements the <see cref="IQueueMessageItem{TBody}"/> interface, allowing for access to the message's body and metadata. This interface can be implemented by classes that interact with different queueing systems, providing a consistent way to handle pulled messages regardless of the underlying implementation.
/// </summary>
/// <typeparam name="TMessageItem">The type of the message item.</typeparam>
/// <typeparam name="TBody">The type of the message body.</typeparam>
public interface IQueuePullResult<TMessageItem, TBody>
    where TMessageItem : IQueueMessageItem<TBody>
{
    /// <summary>
    /// Gets the count of messages that were pulled from the queue. This property provides information about how many messages were retrieved in the pull operation, allowing for further processing or handling of the messages as needed.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Gets an enumerable collection of message items that were pulled from the queue. Each message item is of type TMessageItem, which implements the <see cref="IQueueMessageItem{TBody}"/> interface, allowing for access to the message's body and metadata. This property provides a way to iterate over the pulled messages and process them as needed.
    /// </summary>
    IEnumerable<TMessageItem> Messages { get; }
}
