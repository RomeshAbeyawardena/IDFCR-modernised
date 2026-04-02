using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.Abstractions.DatabaseUpdater.Commands.Database.Migrate;

/// <summary>
/// Represents a command for applying pending database migrations in the CLI. This command is a subcommand of the DatabaseRootMigrateCommand and is responsible for executing any pending migrations that have not yet been applied to the database. It is designed to be injectable, allowing for dependency injection of services required for database migration operations. The command is identified by the name "apply" and can be invoked using the command name "database migrate apply" in the CLI.
/// </summary>
/// <param name="serviceProvider">The service provider for dependency injection.</param>
/// <param name="targetDatabaseConfiguration">The target database configuration.</param>
/// <param name="managedStream">The managed stream for output.</param>
[FeatureCommand(DatabaseRootMigrateCommand.Prefix, CommandName)]
public class DatabaseMigrateApplyCommand(IServiceProvider serviceProvider, ITargetDatabaseConfiguration targetDatabaseConfiguration, IManagedStream managedStream)
    : InjectableCommandOperationBase<DatabaseRootMigrateCommand>(serviceProvider, DatabaseRootMigrateCommand.Prefix, CommandName, typeof(DatabaseRootMigrateCommand))
{
    public const string CommandName = "apply";

    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        if (Services.GetRequiredService(targetDatabaseConfiguration.DbContextType) is not DbContext context)
        {
            await managedStream.Out.WriteLineAsync(@$"The specified DbContext type '{targetDatabaseConfiguration.DbContextType.FullName}' could not be resolved from the service provider. 
                Please ensure that it is registered correctly.", cancellationToken);
            return;
        }

        try
        {
            await managedStream.Out.WriteLineAsync("Applying pending migrations...", cancellationToken);
            await context.Database.MigrateAsync(cancellationToken);
            await managedStream.Out.WriteLineAsync("Database migration completed successfully.", cancellationToken);
        }
        catch (Exception ex)
        {
            await managedStream.Out.WriteLineAsync($"An error occurred during database migration: {ex.Message}", cancellationToken);
        }
    }
}
