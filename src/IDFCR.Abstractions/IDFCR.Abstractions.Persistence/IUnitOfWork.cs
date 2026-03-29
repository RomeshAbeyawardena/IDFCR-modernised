namespace IDFCR.Abstractions.Persistence;

/// <summary>
/// Defines a contract for coordinating the writing of changes to a data store as a single unit of work.
/// </summary>
/// <remarks>Implementations of this interface typically encapsulate a set of operations that should be committed
/// together, ensuring consistency and transactional integrity. The unit of work pattern is commonly used to group
/// multiple changes and persist them in a single operation.</remarks>
public interface IUnitOfWork
{
    /// <summary>
    /// Asynchronously saves all changes made in this context to the underlying database.
    /// </summary>
    /// <remarks>This method does not guarantee that all changes are persisted if an error occurs during the
    /// save operation. If the cancellation token is triggered before the operation completes, the task is canceled and
    /// no changes are saved.</remarks>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous save operation.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries
    /// written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
