namespace IDFCR.Abstractions.Persistence;

/// <summary>
/// Represents a unit of work that supports transactional operations, allowing multiple changes to be committed or rolled back as a single transaction.
/// </summary>
public interface ITransactionalUnitOfWork : IUnitOfWork
{
    /// <summary>
    /// Begins a new transaction or joins an existing transaction if one is already in progress. If a transaction is provided, the unit of work will participate in that transaction; otherwise, it will create a new transaction for the operations performed within this unit of work.
    /// </summary>
    /// <returns>The transaction that was started or joined.</returns>
    IDbTransaction BeginTransaction();

    /// <summary>
    /// Commits all pending changes to the underlying data store as a single atomic operation. If the operation fails, no changes are saved. This method is typically used in unit of work or transactional scenarios to ensure data consistency.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <param name="transaction">The transaction to commit.</param>
    /// <returns>A task that represents the asynchronous commit operation.</returns>
    Task CommitAsync(IDbTransaction transaction, CancellationToken cancellationToken);

    /// <summary>
    /// Commits the current transaction, ensuring that all changes made within the unit of work are persisted to the database. If an error occurs during the commit operation, the transaction will be rolled back to maintain data integrity.
    /// </summary>
    /// <param name="transaction"></param>
    void Commit(IDbTransaction transaction);

    /// <summary>
    /// Rolls back all pending changes to the underlying data store, discarding any modifications made since the last commit. This method is typically used in unit of work or transactional scenarios to undo changes when an error occurs or when the operations performed within the transaction need to be discarded for any reason.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous rollback operation.</returns>
    Task RollbackAsync(IDbTransaction transaction, CancellationToken cancellationToken);

    /// <summary>
    /// Rolls back the current transaction, undoing all changes made within the unit of work. This method should be called if an error occurs during the commit operation or if the operations performed within the unit of work need to be discarded for any reason.
    /// </summary>
    /// <param name="transaction">The transaction to roll back.</param>
    void Rollback(IDbTransaction transaction);
}