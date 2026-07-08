
using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Outbox;

/// <summary>
/// Represents a reader for retrieving outbox messages.
/// </summary>
public interface IOutboxReader
{
    /// <summary>
    /// Retrieves a paged collection of outbox messages asynchronously based on the specified query parameters.
    /// </summary>
    /// <param name="request">The query parameters for retrieving outbox messages.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing a paged collection of outbox messages.</returns>
    Task<IPagedUnitResult<IOutboxEntity>> GetMessagesAsync(
        IPagedQuery request,
        CancellationToken cancellationToken);
}

/// <summary>
/// Represents a reader for retrieving outbox messages of a specific type.
/// </summary>
/// <typeparam name="TMessage">The type of outbox message.</typeparam>
/// <typeparam name="TPagedQuery">The type of the paged query.</typeparam>
public interface IOutboxReader<TMessage, TPagedQuery> : IOutboxReader
    where TMessage : IOutboxEntity
    where TPagedQuery : IPagedQuery
{
    /// <summary>
    /// Retrieves a paged collection of outbox messages of a specific type asynchronously based on the specified query parameters.
    /// </summary>
    /// <param name="request">The query parameters for retrieving outbox messages of a specific type.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing a paged collection of outbox messages of a specific type.</returns>
    Task<IPagedUnitResult<TMessage>> GetMessagesAsync(
        TPagedQuery request,
        CancellationToken cancellationToken);
}