using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;

namespace IDFCR.Abstractions.DatabaseUpdater.Commands.Database.Migrations;

/// <summary>
/// Represents a command for listing pending database migrations in the CLI. This command is a subcommand of the DatabaseMigrateCommand and is responsible for retrieving and displaying any pending migrations that have not yet been applied to the database. It is designed to be injectable, allowing for dependency injection of services required for database migration operations. The command is identified by the name "list" and can be invoked using the command name "database migrate list" in the CLI.
/// </summary>
/// <param name="serviceProvider">The service provider for dependency injection.</param>
/// <param name="databaseFascade">The database fascade for managing database migrations.</param>
/// <param name="managedStream">The managed stream for output.</param>
[FeatureCommand(DatabaseMigrationsRootCommand.Prefix, CommandName)]
public class ListDatabaseMigrationsCommand(IServiceProvider serviceProvider, IDatabaseFascade databaseFascade, IManagedStream managedStream)
    : InjectableCommandOperationBase<DatabaseMigrationsRootCommand>(serviceProvider, DatabaseMigrationsRootCommand.Prefix, CommandName, typeof(DatabaseMigrationsRootCommand))
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
        try
        {
            var migrations = await databaseFascade.GetPendingMigrationsAsync(cancellationToken);

            if (!migrations.Any())
            {
                await managedStream.Out.WriteLineAsync("No pending migrations found. The database is already up to date.", cancellationToken);
                return;
            }

            await managedStream.Out.WriteLineAsync("Pending Migrations:", cancellationToken);
            foreach (var migration in migrations)
            {
                await managedStream.Out.WriteLineAsync($"\t- {migration}", cancellationToken);
            }

        }
        catch (Exception ex)
        {
            await managedStream.Error.WriteLineAsync($"An error occurred during database migration: {ex.Message}", cancellationToken);
        }
    }
}
