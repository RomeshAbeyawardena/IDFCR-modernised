using IDFCR.Abstractions.Outbox;
using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Results.Extensions;

namespace IDFCR.Outbox.Extensions.Dispatchers;

internal class DefaultOutboxDispatcherFactory<TMessage, TPagedQuery>(IEnumerable<IOutboxDispatcher<TMessage, TPagedQuery>> dispatchers) 
    : IOutboxDispatcherFactory<TMessage, TPagedQuery>
    where TMessage : IOutboxEntity
    where TPagedQuery : IPagedQuery
{
    public async Task PushAsync(IPagedUnitResult<TMessage> messages, CancellationToken cancellationToken)
    {
        foreach(var dispatcher in dispatchers)
        {
            await dispatcher.PushAsync(messages, cancellationToken);
        }
    }

    Task IOutboxDispatcher.PushAsync(IPagedUnitResult<IOutboxEntity> messages, CancellationToken cancellationToken)
    {
        var collection = messages.AsCollection<TMessage>();
        return PushAsync(collection.AsPaged(messages.PagedQuery), cancellationToken);
    }
}

///<inheritdoc cref="IOutboxDispatcher{TMessage, TPagedQuery}" />
public abstract class OutboxDispatcherBase<TMessage, TPagedQuery>(IOutboxPublisher<TMessage> publisher) 
    : IOutboxDispatcher<TMessage, TPagedQuery>
    where TMessage : IOutboxEntity
    where TPagedQuery : IPagedQuery
{
    /// <summary>
    /// This method is called when messages are pushed to the dispatcher. It can be overridden to perform additional processing before the messages are handled by the publisher.
    /// </summary>
    /// <param name="messages">The messages being pushed to the dispatcher.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual Task OnPushAsync(IPagedUnitResult<TMessage> messages, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    ///<inheritdoc />
    public async Task PushAsync(IPagedUnitResult<TMessage> messages, CancellationToken cancellationToken)
    {
        if (messages.HasValue)
        {
            await OnPushAsync(messages, cancellationToken).ConfigureAwait(false);
            await publisher.HandleAsync(messages.Result, cancellationToken);
        }
    }

    Task IOutboxDispatcher.PushAsync(IPagedUnitResult<IOutboxEntity> messages, CancellationToken cancellationToken)
    {
        var collection = messages.AsCollection<TMessage>();
        return PushAsync(collection.AsPaged(messages.PagedQuery), cancellationToken);
    }
}
