using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.Abstractions.DatabaseUpdater.Commands.Database.Migrate;

/// <summary>
/// Represents a command for listing pending database migrations in the CLI. This command is a subcommand of the DatabaseMigrateCommand and is responsible for retrieving and displaying any pending migrations that have not yet been applied to the database. It is designed to be injectable, allowing for dependency injection of services required for database migration operations. The command is identified by the name "list" and can be invoked using the command name "database migrate list" in the CLI.
/// </summary>
/// <param name="serviceProvider">The service provider for dependency injection.</param>
/// <param name="targetDatabaseConfiguration">The target database configuration.</param>
/// <param name="managedStream">The managed stream for output.</param>
[FeatureCommand(DatabaseRootMigrateCommand.Prefix, CommandName)]
public class DatabaseMigrateListCommand(IServiceProvider serviceProvider, ITargetDatabaseConfiguration targetDatabaseConfiguration, IManagedStream managedStream)
    : InjectableCommandOperationBase<DatabaseRootMigrateCommand>(serviceProvider, DatabaseRootCommand.Prefix, CommandName, typeof(DatabaseRootMigrateCommand))
{
    /// <summary>
    /// Defines the command name for the database migration list command in the CLI. This command name is used to identify the command when invoked in the CLI environment. By using a clear and descriptive command name, developers can ensure that users understand the purpose of the command and can easily execute it to list pending database migrations. The command name "list" indicates that this command is responsible for listing pending migrations within the CLI, allowing users to see which migrations are awaiting application to the database.
    /// </summary>
    public const string CommandName = "list";

    /// <summary>
    /// Invokes the database migration command when the context is owned.
    /// </summary>
    /// <param name="command">The command arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var isListRequest = Parameters!.TryGetValue("list", out var list) && list.IsFlag;
        var isApplyRequest = Parameters!.TryGetValue("apply", out var apply) && apply.IsFlag;

        if (Services.GetRequiredService(targetDatabaseConfiguration.DbContextType) is not DbContext context)
        {
            await managedStream.Out.WriteLineAsync($"The specified DbContext type '{targetDatabaseConfiguration.DbContextType.FullName}' could not be resolved from the service provider. Please ensure that it is registered correctly.", cancellationToken);
            return;
        }

        try
        {
            var migrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);

            if (!migrations.Any())
            {
                await managedStream.Out.WriteLineAsync("No pending migrations found. The database is already up to date.", cancellationToken);
                return;
            }

            if (isListRequest)
            {
                await managedStream.Out.WriteLineAsync("Pending Migrations:", cancellationToken);
                foreach (var migration in migrations)
                {
                    await managedStream.Out.WriteLineAsync($"\t- {migration}", cancellationToken);
                }
            }

            if (isApplyRequest)
            {
                await managedStream.Out.WriteLineAsync("Applying pending migrations...", cancellationToken);
                await context.Database.MigrateAsync(cancellationToken);
                await managedStream.Out.WriteLineAsync("Database migration completed successfully.", cancellationToken);
            }
        }
        catch (Exception ex)
        {
            await managedStream.Out.WriteLineAsync($"An error occurred during database migration: {ex.Message}", cancellationToken);
        }
    }
}
