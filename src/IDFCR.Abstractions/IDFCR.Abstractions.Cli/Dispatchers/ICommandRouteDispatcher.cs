using IDFCR.Abstractions.Cli.Operations;

namespace IDFCR.Abstractions.Cli.Dispatchers;

/// <summary>
/// Represents a contract for a command route dispatcher, responsible for determining and executing the appropriate command operations based on a given set of commands. This interface defines methods for retrieving the relevant operations that can execute the provided commands and for executing those operations asynchronously, allowing for flexible command processing within a CLI application.
/// </summary>
public interface ICommandRouteDispatcher
{
    /// <summary>
    /// Sets a value indicating whether the dispatcher should list the operations being executed. When set to true, the dispatcher may output information about the operations being invoked for the provided commands, which can be useful for debugging or informational purposes. The setter is private to allow external configuration while preventing modification from within the class itself.
    /// </summary>
    bool ListOperations { set; }

    /// <summary>
    /// Defines a method for retrieving a collection of injectable command operations that are capable of executing the provided commands. This method takes an enumerable of strings representing the commands and returns an enumerable of IInjectableCommandOperation instances that can handle those commands. The returned operations may be used to execute the commands in a structured and efficient manner, allowing for dynamic command processing based on user input or other factors.
    /// </summary>
    /// <param name="commands"></param>
    /// <returns></returns>
    IEnumerable<IInjectableCommandOperation> GetOperations(IEnumerable<string> commands);

    /// <summary>
    /// Defines an asynchronous method for executing the command operations corresponding to the provided commands. This method takes an enumerable of strings representing the commands and a cancellation token to support cooperative cancellation. The method is responsible for invoking the appropriate operations based on the commands, allowing for dynamic execution of command logic within a CLI application. The asynchronous nature of the method enables non-blocking execution, improving responsiveness and scalability when processing commands.
    /// </summary>
    /// <param name="commands">The collection of commands to be executed.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteAsync(IEnumerable<string> commands, CancellationToken cancellationToken);
}
