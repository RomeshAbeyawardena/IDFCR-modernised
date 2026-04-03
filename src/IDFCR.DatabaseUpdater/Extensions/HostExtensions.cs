using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.DatabaseUpdater;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IDFCR.DatabaseUpdater.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="IHost"/> interface, providing additional functionality related to database updating operations. These extensions may include methods for configuring services, managing database migrations, or other operations that enhance the capabilities of the host in the context of database management and updates. By utilizing these extensions, developers can easily integrate database updating features into their applications while maintaining a clean and modular codebase.
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Defines an extension method for the <see cref="IHost"/> interface that configures the host to support database updating operations. This method takes an instance of <see cref="ITargetDatabaseConfiguration"/> to specify the target database configuration, a collection of command-line arguments, and optional parameters for listing operations and cancellation. It also accepts an array of assemblies to scan for database update operations. The method builds and runs the host with the specified configuration, allowing it to execute database update commands based on the provided arguments. The returned <see cref="IDisposable"/> allows for proper disposal of the host resources when they are no longer needed.
    /// <para>
    /// You will need to ensure that <see cref="DbContext"/> is properly configured in the target database configuration, and that the assemblies provided for scanning contain the necessary implementations for database update operations. This method is designed to facilitate the integration of database updating capabilities into applications, making it easier to manage and execute database migrations and updates through a command-line interface. The utility does not assume how the DbContext is configured, allowing for flexibility in how developers set up their database contexts and update operations. By leveraging this extension method, developers can streamline the process of managing database updates and migrations within their applications, ensuring that they can easily maintain and evolve their database schemas as needed.
    /// </para>
    /// </summary>
    /// <param name="configurationInstance">The target database configuration instance.</param>
    /// <param name="args">A collection of command-line arguments.</param>
    /// <param name="listOperations">A boolean indicating whether to list available operations.</param>
    /// <param name="configureServices">An optional action to configure additional services in the host.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation.</param>
    /// <param name="assembliesToScan">An array of assemblies to scan for database update operations.</param>
    /// <returns>An <see cref="IDisposable"/> representing the configured host.</returns>
    public static async Task<IDisposable> ConfigureDatabaseUpdaterHost(ITargetDatabaseConfiguration configurationInstance, 
        IEnumerable<string> args, bool listOperations = false, Action<IServiceCollection>? configureServices = null,
        CancellationToken? cancellationToken = null,
        params System.Reflection.Assembly[] assembliesToScan)
    {
        var hostBuilder = new HostBuilder();
        hostBuilder.ConfigureServices((hostContext, services) => services
            .ConfigureDatabaseUpdater(configurationInstance, assembliesToScan));
        
        if(configureServices is not null)
        {
            hostBuilder.ConfigureServices(configureServices);
        }

        var host = hostBuilder.Build();

        await host.RunCommandsAsync(args, listOperations, cancellationToken.GetValueOrDefault(CancellationToken.None));

        return host;
    }
}
