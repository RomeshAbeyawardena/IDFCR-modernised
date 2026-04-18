using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IDFCR.Abstractions.Cli.Dispatchers;

/// <summary>
/// Represents a dispatcher responsible for routing command strings to their corresponding command operations. This class manages the
/// </summary>
/// <param name="host"></param>
public class CommandRouteDispatcher(IHost host) : IDisposable
{
    private IServiceScope? _serviceScope;

    /// <summary>
    /// Gets or sets a value indicating whether the dispatcher should list available operations when executing commands. When set to true, the dispatcher may provide additional information about the operations being executed, such as their names or descriptions, which can be useful for debugging or informational purposes.
    /// </summary>
    public bool ListOperations { private get; set; }

    /// <summary>
    /// Defines a method to begin the execution of a sequence of command strings. This method creates a new service scope for each execution, ensuring that any dependencies required by the command operations are properly resolved and disposed of after execution. The method takes an enumerable of command strings and an optional cancellation token, allowing for graceful cancellation of the command execution process if needed.
    /// </summary>
    /// <param name="commands">The sequence of command strings to be executed.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the execution.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task BeginAsync(IEnumerable<string> commands, CancellationToken? cancellationToken = null)
    {
        if (_serviceScope is not null)
        {
            _serviceScope?.Dispose();
        }

        _serviceScope = host.Services.CreateScope();
        var dispatcher = _serviceScope.ServiceProvider.GetRequiredService<ICommandRouteDispatcher>();
        dispatcher.ListOperations = ListOperations;
        return dispatcher.ExecuteAsync(commands, cancellationToken.GetValueOrDefault(CancellationToken.None));
    }

    /// <summary>
    /// Disposes of the resources used by the CommandRouteDispatcher. This method ensures that any active service scope is properly disposed of, releasing any resources associated with it. After disposing of the service scope, it suppresses finalization to optimize garbage collection.
    /// </summary>
    public void Dispose()
    {
        _serviceScope?.Dispose();
        GC.SuppressFinalize(this);
    }
}
