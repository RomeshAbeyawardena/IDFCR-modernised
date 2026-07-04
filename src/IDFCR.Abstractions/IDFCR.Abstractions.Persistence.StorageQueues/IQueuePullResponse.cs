namespace IDFCR.Abstractions.Persistence.StorageQueues;

/// <summary>
/// Represents a response from a queue pull operation, containing the result of the operation, any errors that occurred, and any messages associated with the operation. The interface is generic, allowing for different types of results, message items, errors, and message bodies to be specified. This allows for flexibility in handling different queueing systems and message formats.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <typeparam name="TMessageItem">The type of the message item.</typeparam>
/// <typeparam name="TError">The type of the error.</typeparam>
/// <typeparam name="TBody">The type of the message body.</typeparam>
public interface IQueuePullResponse<TResult, TMessageItem, TError, TBody>
    where TMessageItem : IQueueMessageItem<TBody>
    where TResult : IQueuePullResult<TMessageItem, TBody>
{
    /// <summary>
    /// Gets a value indicating whether the queue pull operation was successful. If true, the operation was successful and the Result property contains the result of the operation. If false, the operation failed and the Errors property contains any errors that occurred during the operation.
    /// </summary>
    bool Success { get; }
    /// <summary>
    /// Gets a collection of errors that occurred during the queue pull operation. If the Success property is false, this collection contains the errors that caused the operation to fail. If the Success property is true, this collection is empty.
    /// </summary>
    IEnumerable<TError> Errors { get; }
    /// <summary>
    /// Gets a collection of messages associated with the queue pull operation. These messages can provide additional context or information about the operation, such as warnings or informational messages. The Messages property is always populated, regardless of whether the Success property is true or false.
    /// </summary>
    IEnumerable<string> Messages { get; }
    /// <summary>
    /// Gets the result of the queue pull operation. If the Success property is true, this property contains the result of the operation, including any messages that were pulled from the queue. If the Success property is false, this property is null.
    /// </summary>
    TResult? Result { get; }
}
