using IDFCR.Abstractions.Outbox;
using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Results.Extensions;

namespace IDFCR.Outbox.Extensions.Dispatchers;

/// <inheritdoc cref="IOutboxReader{TMessage, TPagedQuery}" />
public abstract class OutboxReaderBase<TMessage, TPagedQuery>(string name) : IOutboxReader<TMessage, TPagedQuery>
    where TMessage : IOutboxEntity
    where TPagedQuery : IPagedQuery
{
    ///<inheritdoc />
    public abstract Task<IPagedUnitResult<TMessage>> GetMessagesAsync(TPagedQuery request, CancellationToken cancellationToken);

    ///<inheritdoc />
    public string Name { get; } = name;

    async Task<IPagedUnitResult<IOutboxEntity>> IOutboxReader.GetMessagesAsync(IPagedQuery request, CancellationToken cancellationToken)
    {
        var messages = await GetMessagesAsync((TPagedQuery)request, cancellationToken);

        var outboxEntities = messages.AsCollection<IOutboxEntity>();

        return outboxEntities.AsPaged(messages.PagedQuery);
    }
}