using IDFCR.Abstractions.Outbox;

namespace IDFCR.Outbox.Extensions.Dispatchers;

///<inheritdoc cref="IOutboxPublisher{TMessage}" />
public abstract class OutboxPublisherBase<TMessage> 
    : IOutboxPublisher<TMessage>
    where TMessage : IOutboxEntity
{
    ///<inheritdoc />
    public abstract Task HandleAsync(IEnumerable<TMessage> messages, CancellationToken cancellationToken);

    Task IOutboxPublisher.HandleAsync(IEnumerable<IOutboxEntity> messages, CancellationToken cancellationToken)
    {
        return HandleAsync(messages.Cast<TMessage>(), cancellationToken);
    }
}

public abstract class OutboxReader
{

}