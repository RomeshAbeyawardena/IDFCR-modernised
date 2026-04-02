using IDFCR.Abstractions.Cli.Operations;

namespace IDFCR.Abstractions.DatabaseUpdater.Commands;

/// <summary>
/// Represents the root command for database operations in the CLI. This command serves as the entry point for all database-related subcommands and operations. It is designed to be injectable, allowing for dependency injection of services required for database operations. The command is identified by the prefix "database" and can be invoked using the command name "database" in the CLI.
/// </summary>
/// <param name="serviceProvider"></param>
public class DatabaseRootCommand(IServiceProvider serviceProvider) : InjectableCommandOperationRootBase<DatabaseRootCommand>(serviceProvider, Prefix, CommandName, null)
{
    /// <summary>
    /// Defines the prefix for the database root command in the CLI. This prefix is used to identify the command and its associated subcommands when invoked in the CLI environment. By using a consistent prefix, developers can ensure that all database-related commands are organized under a common namespace, making it easier for users to discover and execute database operations within the CLI. The prefix "database" indicates that this command is related to database operations and serves as the root for all subsequent database commands.
    /// </summary>
    public const string Prefix = "database";
    /// <summary>
    /// Defines the command name for the database root command in the CLI. This command name is used to identify the command when invoked in the CLI environment. By using a clear and descriptive command name, developers can ensure that users understand the purpose of the command and can easily execute it to access database-related operations. The command name "database" indicates that this command serves as the main entry point for all database operations within the CLI.
    /// </summary>
    public const string CommandName = "database";
}
