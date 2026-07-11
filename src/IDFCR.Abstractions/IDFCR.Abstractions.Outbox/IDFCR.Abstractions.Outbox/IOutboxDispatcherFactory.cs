using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Outbox;

/// <summary>
/// Represents a factory for sending outbox messages of a specific type to multiple dispatchers.
/// </summary>
/// <typeparam name="TMessage">The type of outbox message.</typeparam>
/// <typeparam name="TPagedQuery">The type of the paged query.</typeparam>
public interface IOutboxDispatcherFactory<TMessage, TPagedQuery> : IOutboxDispatcher
    where TMessage : IOutboxEntity
    where TPagedQuery : IPagedQuery
{
    /// <summary>
    /// Pushes a collection of outbox messages of a specific type asynchronously to multiple dispatchers.
    /// </summary>
    /// <param name="messages">The collection of outbox messages to push.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PushAsync(IPagedUnitResult<TMessage> messages, CancellationToken cancellationToken);
}