using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Outbox;

/// <summary>
/// Represents a handler for processing outbox messages.
/// </summary>
public interface IOutboxPublisher
{
    /// <summary>
    /// Handles a collection of outbox messages asynchronously.
    /// </summary>
    /// <param name="messages">The collection of outbox messages to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HandleAsync(
        IEnumerable<IOutboxEntity> messages,
        CancellationToken cancellationToken);
}

/// <summary>
/// Represents a handler for processing outbox messages of a specific type.
/// </summary>
/// <typeparam name="TMessage">The type of outbox message.</typeparam>
public interface IOutboxPublisher<TMessage> : IOutboxPublisher
    where TMessage : IOutboxEntity
{
    /// <summary>
    /// Handles a collection of outbox messages asynchronously.
    /// </summary>
    /// <param name="messages">The collection of outbox messages to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HandleAsync(
        IEnumerable<TMessage> messages,
        CancellationToken cancellationToken);
}
