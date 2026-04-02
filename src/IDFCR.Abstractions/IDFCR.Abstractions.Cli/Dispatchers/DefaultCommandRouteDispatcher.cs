using IDFCR.Abstractions.Cli.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.Abstractions.Cli.Dispatchers;

/// <summary>
/// Represents the default implementation of the <see cref="ICommandRouteDispatcher"/> interface, responsible for dispatching command routes to their corresponding operations based on the provided commands. This dispatcher utilizes dependency injection to retrieve available command operation roots and determines which operations can execute the given commands, allowing for flexible and extensible command processing within a CLI application.
/// </summary>
/// <param name="services"></param>
public sealed class DefaultCommandRouteDispatcher(IServiceProvider services) : ICommandRouteDispatcher
{
    /// <summary>
    /// Gets or sets a value indicating whether the dispatcher should list the operations being executed. When set to true, the dispatcher may output information about the operations being invoked for the provided commands, which can be useful for debugging or informational purposes. The setter is private to allow external configuration while preventing modification from within the class itself.s
    /// </summary>
    public bool ListOperations { private get; set; }

    /// <summary>
    /// Defines an asynchronous method to execute the command operations corresponding to the provided commands. The method retrieves the relevant operations based on the commands and invokes them sequentially, passing along the cancellation token to support cooperative cancellation. The execution of each operation may involve processing the commands and performing the necessary actions defined by the operation's implementation.
    /// </summary>
    /// <param name="commands">The collection of commands to be executed.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ExecuteAsync(IEnumerable<string> commands, CancellationToken cancellationToken)
    {
        var operations = GetOperations(commands);
        foreach (var operation in operations)
        {
            operation.ListOperations = ListOperations;
            await operation.InvokeAsync(commands, cancellationToken);
        }
    }

    /// <summary>
    /// Defines a method to retrieve the command operations that can execute the provided commands. The method queries the registered services for instances of <see cref="IInjectableCommandOperationRoot"/> and filters them based on their ability to execute the given commands. The resulting collection of operations is returned, allowing the dispatcher to determine which operations should be invoked for the specified commands.
    /// </summary>
    /// <param name="commands"></param>
    /// <returns></returns>
    public IEnumerable<IInjectableCommandOperation> GetOperations(IEnumerable<string> commands)
    {
#pragma warning disable CA2263 //extension methods aren't supported by MOQ
        var s = services.GetServices(typeof(IInjectableCommandOperationRoot))
            .Select(x => x as IInjectableCommandOperationRoot)
            .ToArray();

        List<IInjectableCommandOperation> operations = [];

        foreach (var command in commands)
        {
            operations.AddRange(s.Where(x => x != null && x.CanExecute(command))!);
        }
        return operations;
#pragma warning restore
    }
}
