
using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Outbox;

/// <summary>
/// Represents a factory for resolving outbox readers for a specific message type.
/// </summary>
/// <typeparam name="TMessage">The type of outbox message.</typeparam>
public interface IOutboxReaderFactory<TMessage>
    where TMessage : IOutboxEntity
{
    /// <summary>
    /// Gets a collection of compatible outbox readers for the specified paged query type.
    /// </summary>
    /// <typeparam name="TPagedQuery">The type of the paged query.</typeparam>
    /// <returns>A collection of compatible outbox readers.</returns>
    IEnumerable<IOutboxReader<TMessage, TPagedQuery>> GetCompatibleReaders<TPagedQuery>()
        where TPagedQuery : IPagedQuery;
}
