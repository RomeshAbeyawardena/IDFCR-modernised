using IDFCR.Abstractions.DatabaseUpdater;
using Microsoft.EntityFrameworkCore;

namespace IDFCR.DatabaseUpdater.Extensions;

/// <summary>
/// Defines extension methods for configuring the target database in the CLI. This class provides a fluent API for specifying the target DbContext type that will be used for database operations within the CLI environment. By using these extension methods, developers can easily configure their services to ensure that the correct DbContext is associated with their database-related commands and operations, allowing for seamless integration and execution of database tasks in the CLI.
/// </summary>
public static class TargetDatabaseConfiguration
{
    /// <summary>
    /// Creates a new instance of the target database configuration for the specified DbContext type. This method allows developers to specify the DbContext type that will be used as the target database for CLI operations. By providing the DbContext type, the CLI can identify and utilize the appropriate database context for executing commands and performing database-related tasks, ensuring that the correct database is targeted during operations.
    /// </summary>
    /// <typeparam name="TDbContextType">The type of the DbContext to use as the target database.</typeparam>
    /// <returns>An instance of <see cref="ITargetDatabaseConfiguration"/> for the specified DbContext type.</returns>
    public static ITargetDatabaseConfiguration Create<TDbContextType>() where TDbContextType : DbContext
    {
        return new TargetDatabaseConfigurationDescriptor<TDbContextType>();
    }
}
