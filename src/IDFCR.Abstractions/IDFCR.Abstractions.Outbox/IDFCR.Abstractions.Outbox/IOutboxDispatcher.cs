using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Outbox;

/// <summary>
/// Represents a dispatcher for retrieving outbox messages.
/// </summary>
public interface IOutboxDispatcher
{
    /// <summary>
    /// Pushes a collection of outbox messages asynchronously.
    /// </summary>
    /// <param name="messages">The collection of outbox messages to push.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PushAsync(IPagedUnitResult<IOutboxEntity> messages, CancellationToken cancellationToken);
}

/// <summary>
/// Represents a dispatcher for retrieving outbox messages of a specific type.
/// </summary>
/// <typeparam name="TMessage">The type of outbox message.</typeparam>
/// <typeparam name="TPagedQuery">The type of the paged query.</typeparam>
public interface IOutboxDispatcher<TMessage, TPagedQuery> : IOutboxDispatcher
    where TMessage : IOutboxEntity
    where TPagedQuery : IPagedQuery
{
    /// <summary>
    /// Pushes a collection of outbox messages of a specific type asynchronously.
    /// </summary>
    /// <param name="messages">The collection of outbox messages to push.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PushAsync(IPagedUnitResult<TMessage> messages, CancellationToken cancellationToken);
}
