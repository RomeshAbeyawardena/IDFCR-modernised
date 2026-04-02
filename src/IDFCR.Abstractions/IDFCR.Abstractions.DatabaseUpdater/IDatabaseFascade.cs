namespace IDFCR.Abstractions.DatabaseUpdater;

/// <summary>
/// Represents a facade for database operations, specifically for managing database migrations. This interface defines methods for retrieving pending migrations and applying them to the database. It abstracts the underlying implementation of database migration management, allowing for flexibility in how migrations are handled while providing a consistent interface for interacting with the database migration process. The methods defined in this interface are asynchronous, enabling efficient handling of potentially long-running database operations without blocking the calling thread.
/// </summary>
public interface IDatabaseFascade
{
    /// <summary>
    /// Retrieves a list of pending database migrations that have not yet been applied to the database. This method is asynchronous and returns an enumerable collection of strings, where each string represents the name of a pending migration. The method takes a cancellation token as a parameter, allowing the operation to be cancelled if needed. By providing this method, the interface allows clients to check for pending migrations before attempting to apply them, enabling better control over the database migration process and allowing for informed decision-making regarding when to apply migrations.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of pending migration names.</returns>
    Task<IEnumerable<string>> GetPendingMigrationsAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Applies all pending database migrations. This method is asynchronous and takes a cancellation token as a parameter, allowing the operation to be cancelled if needed. By providing this method, the interface allows clients to apply migrations in a controlled and consistent manner, ensuring that the database schema is up-to-date.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>   
    Task MigrateAsync(CancellationToken cancellationToken);
}
