using IDFCR.Abstractions.Cli.Dispatchers;
using Microsoft.Extensions.Hosting;

namespace IDFCR.Abstractions.Cli.Extensions;

/// <summary>
/// Defines extension methods for the IHost interface to run CLI commands using a CommandRouteDispatcher. This class provides a method to execute commands asynchronously by passing command-line arguments and an optional cancellation token. The RunCommandsAsync method creates an instance of the CommandRouteDispatcher, which is responsible for dispatching commands based on the provided arguments, and then begins the command execution process. By using this extension method, developers can easily integrate CLI command execution into their applications that utilize the IHost interface for hosting and dependency injection.
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Runs CLI commands asynchronously using a CommandRouteDispatcher. This method takes an IHost instance, a collection of command-line arguments, an optional flag to list operations, and an optional cancellation token. It creates an instance of the CommandRouteDispatcher, sets the ListOperations property based on the provided flag, and then begins the command execution process by calling the BeginAsync method with the provided arguments and cancellation token. The method returns a Task that represents the asynchronous operation of running the commands. By using this extension method, developers can easily execute CLI commands within their applications that utilize the IHost interface for hosting and dependency injection.
    /// </summary>
    /// <param name="host">The IHost instance used to resolve dependencies and manage the application's lifetime.</param>
    /// <param name="args">A collection of command-line arguments to be passed to the command dispatcher.</param>
    /// <param name="listOperations">A flag indicating whether to list available operations instead of executing a command.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the command execution.</param>
    /// <returns></returns>
    public static Task RunCommandsAsync(this IHost host, IEnumerable<string> args, bool listOperations = false, CancellationToken? cancellationToken = null)
    {
        using var commandDispatcher = new CommandRouteDispatcher(host)
        {
            ListOperations = listOperations
        };

        return commandDispatcher.BeginAsync(args, cancellationToken);
    }
}
