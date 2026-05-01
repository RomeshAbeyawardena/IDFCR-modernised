namespace IDFCR.Abstractions.Persistence;

/// <summary>
/// Represents a database transaction that can be committed or rolled back as a unit of work. This interface provides
/// </summary>
public interface IDbTransaction : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Commits all pending changes to the underlying data store as a single atomic operation. If the operation fails, no changes are saved. This method is typically used in unit of work or transactional scenarios to ensure data consistency.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous commit operation.</returns>
    Task CommitAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Commits all pending changes to the underlying data store as a single atomic operation.
    /// </summary>
    /// <remarks>Call this method to persist any modifications made since the last commit. If the operation
    /// fails, no changes are saved. This method is typically used in unit of work or transactional scenarios to ensure
    /// data consistency.</remarks>
    void Commit();

    /// <summary>
    /// Rolls back all pending changes to the underlying data store, discarding any modifications made since the last commit. This method is typically used in unit of work or transactional scenarios to undo changes when an error occurs or when the operations performed within the transaction need to be discarded for any reason.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous rollback operation.</returns>
    Task RollbackAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Rolls back all pending changes to the underlying data store, discarding any modifications made since the last commit.
    /// </summary>
    void Rollback();
}
