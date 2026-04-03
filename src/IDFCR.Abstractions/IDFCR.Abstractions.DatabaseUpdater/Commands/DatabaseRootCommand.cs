using IDFCR.Abstractions.Cli;
using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;

namespace IDFCR.Abstractions.DatabaseUpdater.Commands;

/// <summary>
/// Represents the root command for database operations in the CLI. This command serves as the entry point for all database-related subcommands and operations. It is designed to be injectable, allowing for dependency injection of services required for database operations. The command is identified by the prefix "database" and can be invoked using the command name "database" in the CLI.
/// </summary>
/// <param name="serviceProvider">The service provider for dependency injection.</param>
/// <param name="timeProvider">The time provider for obtaining the current time.</param>
/// <param name="managedStream">The managed stream for handling CLI input and output.</param>
/// <param name="promptGreeter">The prompt greeter for generating greeting prompts.</param>
public class DatabaseRootCommand(IServiceProvider serviceProvider, TimeProvider timeProvider, IManagedStream managedStream, IPromptGreeter promptGreeter) 
    : InjectableCommandOperationRootBase<DatabaseRootCommand>(serviceProvider, Prefix, CommandName, null)
{
    /// <summary>
    /// Defines the prefix for the database root command in the CLI. This prefix is used to identify the command and its associated subcommands when invoked in the CLI environment. By using a consistent prefix, developers can ensure that all database-related commands are organized under a common namespace, making it easier for users to discover and execute database operations within the CLI. The prefix "database" indicates that this command is related to database operations and serves as the root for all subsequent database commands.
    /// </summary>
    public const string Prefix = "database";
    /// <summary>
    /// Defines the command name for the database root command in the CLI. This command name is used to identify the command when invoked in the CLI environment. By using a clear and descriptive command name, developers can ensure that users understand the purpose of the command and can easily execute it to access database-related operations. The command name "database" indicates that this command serves as the main entry point for all database operations within the CLI.
    /// </summary>
    public const string CommandName = "database";

    /// <summary>
    /// Defines an asynchronous method to invoke the database root command with the provided commands and cancellation token. This method is responsible for generating a greeting prompt using the prompt greeter and writing it to the output stream before invoking the base implementation of the command operation. By generating a greeting prompt, this method enhances the user experience by providing a personalized and engaging introduction when users execute database-related commands in the CLI. The method ensures that the greeting is displayed before any further processing of the commands takes place, creating a welcoming environment for users interacting with database operations in the CLI.
    /// </summary>
    /// <param name="commands">The collection of commands to be executed by the database root command.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override async Task InvokeAsync(IEnumerable<string> commands, CancellationToken cancellationToken)
    {
        await managedStream.Out.WriteLineAsync(promptGreeter.GenerateGreetingPrompt(timeProvider.GetUtcNow()), cancellationToken);
        await base.InvokeAsync(commands, cancellationToken);
    }
}
