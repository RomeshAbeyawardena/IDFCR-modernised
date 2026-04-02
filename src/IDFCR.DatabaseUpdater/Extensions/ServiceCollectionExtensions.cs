using IDFCR.Abstractions.DatabaseUpdater;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.DatabaseUpdater.Extensions;

/// <summary>
/// Provides extension methods for configuring services in the IDFCR.DatabaseUpdater namespace.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures the database updater services by registering the specified target database configuration. This method ensures that the target database configuration is properly registered in the service collection, allowing the CLI to identify and use the correct DbContext type for database operations. It checks if the specified DbContext type is already registered in the service collection and throws an exception if it is not, ensuring that the necessary dependencies are in place for successful database operations.
    /// </summary>
    /// <typeparam name="TTargetDatabaseConfiguration">The type of the target database configuration to register.</typeparam>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configurationInstance">The instance of the target database configuration to register.</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the specified DbContext type is not registered in the service collection.</exception>
    public static IServiceCollection ConfigureDatabaseUpdater<TTargetDatabaseConfiguration>(this IServiceCollection services, ITargetDatabaseConfiguration configurationInstance)
        where TTargetDatabaseConfiguration : class, ITargetDatabaseConfiguration, new()
    {
        if (!services.Any(s => s.ServiceType == configurationInstance.DbContextType))
        {
            throw new InvalidOperationException($"The specified DbContext type '{configurationInstance.DbContextType.FullName}' is not registered in the service collection. Please ensure that the DbContext is registered before configuring the database updater.");
        }

        return services.AddSingleton<ITargetDatabaseConfiguration>();
    }
}
