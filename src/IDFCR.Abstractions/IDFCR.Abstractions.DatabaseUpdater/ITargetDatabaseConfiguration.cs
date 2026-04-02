using Microsoft.EntityFrameworkCore;

namespace IDFCR.Abstractions.DatabaseUpdater;

/// <summary>
/// Represents a configuration interface for specifying the target database context in the CLI. This interface is designed to be implemented by classes that need to define the specific DbContext type that will be used for database operations. By implementing this interface, developers can ensure that their commands or operations are correctly associated with the intended DbContext, allowing for accurate and efficient database interactions within the CLI environment.
/// </summary>
public interface ITargetDatabaseConfiguration
{
    /// <summary>
    /// Gets the type of the target DbContext for database operations. This property is used to identify the specific DbContext that will be targeted for operations such as migrations, updates, or other database-related tasks. The type information provided by this property allows the CLI to determine which DbContext to use when executing commands that require database context, ensuring that the correct database configuration is applied during operations.
    /// </summary>
    Type DbContextType { get; }
}

/// <summary>
/// Represents a configuration interface for specifying the target database context in the CLI. This interface is designed to be implemented by classes that need to define the specific DbContext type that will be used for database operations. By implementing this interface, developers can ensure that their commands or operations are correctly associated with the intended DbContext, allowing for accurate and efficient database interactions within the CLI environment.
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public interface ITargetDatabaseConfiguration<TDbContext> : ITargetDatabaseConfiguration
    where TDbContext : DbContext
{
    /// <summary>
    /// Gets the type of the target DbContext for database operations. This property is used to identify the specific DbContext that will be targeted for operations such as migrations, updates, or other database-related tasks. The type information provided by this property allows the CLI to determine which DbContext to use when executing commands that require database context, ensuring that the correct database configuration is applied during operations.
    /// </summary>
    new Type DbContextType => typeof(TDbContext);
}
