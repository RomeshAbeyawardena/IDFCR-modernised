using IDFCR.Abstractions.Outbox;
using IDFCR.Abstractions.Results;
using IDFCR.Outbox.Extensions.Dispatchers;
using IDFCR.Abstractions.Persistence;

namespace IDFCR.Outbox.EntityFramework;

/// <summary>
/// Represents a base class for reading outbox messages from a repository. This class provides an implementation of the GetMessagesAsync method, which retrieves paged results from the underlying repository based on the provided query.
/// </summary>
/// <typeparam name="TMessage">The type of the outbox message.</typeparam>
/// <typeparam name="TPagedQuery">The type of the paged query.</typeparam>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <param name="name">The name of the outbox reader, typically representing the type of messages it handles.</param>
/// <param name="repository">The repository instance.</param>
public class RepositoryOutboxReaderBase<TMessage, TPagedQuery, TKey>(string name, IRepository<TMessage, TKey> repository) : OutboxReaderBase<TMessage, TPagedQuery>($"{name}RepositoryOutboxReader")
    where TMessage : class, IOutboxEntity
    where TPagedQuery : IPagedQuery
    where TKey : struct
{
    /// <summary>
    /// Retrieves a paged result of outbox messages based on the provided query. This method delegates the retrieval to the underlying repository's GetPagedAsync method, passing the query and cancellation token.
    /// </summary>
    /// <param name="request">The paged query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, containing the paged result of outbox messages.</returns>
    public override async Task<IPagedUnitResult<TMessage>> GetMessagesAsync(TPagedQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetPagedAsync(request, cancellationToken);
    }
}
