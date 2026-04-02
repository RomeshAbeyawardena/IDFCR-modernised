using IDFCR.Abstractions.Cli.StateManagement;

namespace IDFCR.Abstractions.Cli.Operations;

/// <summary>
/// Represents an operation that can be invoked with a command string and supports a state management mechanism through the CommandOperationState class. This interface defines methods for initializing the operation, invoking it with commands, and managing supported operations. It also includes a property to indicate whether the operation should list available operations when invoked without specific commands. This allows for a flexible and extensible command handling system in a CLI application.
/// </summary>
public interface ICommandOperation : IOperation
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation should list available operations when invoked without specific commands. If set to true, invoking the operation without any commands will result in a list of supported operations being displayed. This can be useful for providing users with guidance on available commands and their usage in a CLI application.
    /// </summary>
    bool ListOperations { set; }
    /// <summary>
    /// Defines a method to determine if the operation matches a given name. This method takes a string parameter representing the name to compare against the operation's identifier or command name. The implementation of this method should return true if the provided name matches the operation, allowing for flexible command matching in a CLI application. This can be used to route commands to the appropriate operations based on user input.
    /// </summary>
    /// <param name="name">The name to compare against the operation's identifier or command name.</param>
    /// <returns>True if the provided name matches the operation; otherwise, false.</returns>
    bool Equals(string? name);
    /// <summary>
    /// Initializes the operation with an optional CommandOperationState parameter. This method is responsible for setting up the operation's state and preparing it for invocation. The CommandOperationState can be used to manage the current state of the operation, including any relevant data or context needed for executing commands. If no state is provided, the operation can initialize with a default state or remain uninitialized until invoked. This allows for a flexible initialization process that can accommodate various scenarios in a CLI application.
    /// </summary>
    /// <param name="State"></param>
    void Initialise(CommandOperationState? State = null);
    /// <summary>
    /// Gets the current state of the operation. The CommandOperationState property provides access to the state management mechanism for the operation, allowing for tracking and managing the current context or data associated with the operation. This can be useful for maintaining information across multiple invocations of the operation or for providing feedback to the user based on the current state. The implementation of this property should return the current state of the operation, which can be used by other components in a CLI application to make informed decisions or provide relevant information to the user.
    /// </summary>
    CommandOperationState State { get; }
    /// <summary>
    /// Gets a collection of supported operations that can be invoked through this command operation. The SupportedOperations property provides access to a collection of ICommandOperation instances that represent the various operations that can be executed as part of this command operation. This allows for a hierarchical structure of commands and sub-commands in a CLI application, where each command operation can have its own set of supported operations that can be invoked based on user input. The implementation of this property should return the collection of supported operations, which can be used to determine available commands and their corresponding actions when the main command operation is invoked.
    /// </summary>
    IEnumerable<ICommandOperation> SupportedOperations { get; }
    /// <summary>
    /// Invokes the operation with a given command string and a cancellation token. This method is responsible for executing the logic associated with the command operation based on the provided command string. The command string can be parsed to determine which specific operation to execute from the SupportedOperations collection, or it can be used to perform a specific action directly within the operation. The cancellation token allows for graceful cancellation of the operation if needed, providing a way to handle long-running tasks or user-initiated cancellations in a CLI application. The implementation of this method should ensure that the appropriate logic is executed based on the command string and that any necessary cleanup or state management is performed when the operation is cancelled or completed.
    /// </summary>
    /// <param name="command">The command string to be executed by the operation.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InvokeAsync(string command, CancellationToken cancellationToken);
    /// <summary>
    /// Invokes the operation with a collection of command strings and a cancellation token. This method is responsible for executing the logic associated with the command operation based on the provided collection of command strings. The collection of command strings can be parsed to determine which specific operations to execute from the SupportedOperations collection, or it can be used to perform specific actions directly within the operation. The cancellation token allows for graceful cancellation of the operation if needed, providing a way to handle long-running tasks or user-initiated cancellations in a CLI application. The implementation of this method should ensure that the appropriate logic is executed based on the collection of command strings and that any necessary cleanup or state management is performed when the operation is cancelled or completed.
    /// </summary>
    /// <param name="commands">A collection of command strings to be executed by the operation.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InvokeAsync(IEnumerable<string> commands, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a supported operation to the collection of operations that can be invoked through this command operation. This method allows for dynamically adding new operations to the SupportedOperations collection, enabling extensibility and flexibility in a CLI application. By adding supported operations, you can create a hierarchical structure of commands and sub-commands, allowing users to access a wide range of functionality through a single command operation. The implementation of this method should ensure that the provided ICommandOperation instance is added to the collection of supported operations, making it available for invocation when the main command operation is executed.
    /// </summary>
    /// <param name="operation">The ICommandOperation instance to be added to the collection of supported operations.</param>
    void AddSupportedOperation(ICommandOperation operation);

    /// <summary>
    /// Adds multiple supported operations to the collection of operations that can be invoked through this command operation. This method allows for dynamically adding multiple new operations to the SupportedOperations collection in a single call, enabling extensibility and flexibility in a CLI application. By adding supported operations, you can create a hierarchical structure of commands and sub-commands, allowing users to access a wide range of functionality through a single command operation. The implementation of this method should ensure that each provided ICommandOperation instance is added to the collection of supported operations, making them available for invocation when the main command operation is executed.
    /// </summary>
    /// <param name="operations">An array of ICommandOperation instances to be added to the collection of supported operations.</param>
    void AddSupportedOperations(params ICommandOperation[] operations);
}
