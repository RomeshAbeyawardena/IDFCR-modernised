using IDFCR.Abstractions.Outbox;
using IDFCR.Abstractions.Results;

namespace IDFCR.Outbox.Extensions.Dispatchers;

internal class DefaultOutboxReaderFactory<TMessage>(IEnumerable<IOutboxReader> outboxReaders) : IOutboxReaderFactory<TMessage>
    where TMessage : IOutboxEntity
{
    public IEnumerable<IOutboxReader<TMessage, TPagedQuery>> GetCompatibleReaders<TPagedQuery>() where TPagedQuery : IPagedQuery
    {
        return outboxReaders.OfType<IOutboxReader<TMessage, TPagedQuery>>();
    }
}
