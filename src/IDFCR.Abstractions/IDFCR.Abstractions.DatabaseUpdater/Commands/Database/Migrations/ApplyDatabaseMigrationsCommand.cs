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
[FeatureCommand(DatabaseMigrationsRootCommand.Prefix, CommandName)]
public class ApplyDatabaseMigrationsCommand(IServiceProvider serviceProvider, ITargetDatabaseConfiguration targetDatabaseConfiguration, IManagedStream managedStream)
    : InjectableCommandOperationBase<DatabaseMigrationsRootCommand>(serviceProvider, DatabaseMigrationsRootCommand.Prefix, CommandName, typeof(DatabaseMigrationsRootCommand))
{
    /// <summary>
    /// Defines the command name for applying pending database migrations. This constant is used to identify the command when it is invoked in the CLI. The command name is "apply", and it is a subcommand of the "database migrate" command group. When users execute the command "database migrate apply", this operation will be triggered to apply any pending migrations to the database.
    /// </summary>
    public const string CommandName = "apply";

    /// <summary>
    /// Invoke the database migration command when the context is owned. This method is responsible for applying any pending migrations to the database. It retrieves the specified DbContext from the service provider, checks for pending migrations, and applies them to the database. The method also handles exceptions that may occur during the migration process and provides feedback to the user through the managed stream output.
    /// </summary>
    /// <param name="command">The command arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
