using IDFCR.Abstractions.Cli.Operations;

namespace IDFCR.Abstractions.DatabaseUpdater.Commands.Database.Migrate;

/// <summary>
/// Represents a command for migrating the database in the CLI. This command is a subcommand of the DatabaseRootCommand and is responsible for executing database migration operations. It is designed to be injectable, allowing for dependency injection of services required for database migration. The command is identified by the name "migrate" and can be invoked using the command name "database migrate" in the CLI.
/// </summary>
/// <param name="serviceProvider"></param>
public class DatabaseRootMigrateCommand(IServiceProvider serviceProvider)
    : InjectableCommandOperationRootBase<DatabaseRootMigrateCommand>(serviceProvider, DatabaseRootCommand.Prefix, CommandName, typeof(DatabaseRootCommand))
{
    /// <summary>
    /// Defines the prefix for the database migration command in the CLI. This prefix is used to identify the command and its associated subcommands when invoked in the CLI environment. By using a consistent prefix, developers can ensure that all database migration-related commands are organized under a common namespace, making it easier for users to discover and execute database migration operations within the CLI. The prefix "database-migrate" indicates that this command is related to database migration operations and serves as the root for all subsequent database migration commands within the CLI.
    /// </summary>
    public const string Prefix = $"{DatabaseRootCommand.Prefix}-migrate";
    /// <summary>
    /// Defines the command name for the database migration command in the CLI. This command name is used to identify the command when invoked in the CLI environment. By using a clear and descriptive command name, developers can ensure that users understand the purpose of the command and can easily execute it to perform database migration operations. The command name "migrate" indicates that this command is responsible for executing database migrations within the CLI.
    /// </summary>
    public const string CommandName = "migrate";
    
}