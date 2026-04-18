using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using Microsoft.Extensions.DependencyInjection;


namespace IDFCR.Abstractions.Cli.Operations;

/// <summary>
/// Represents a base class for injectable command operations that extends the CommandOperationBase class and implements the IInjectableCommandOperation interface. This class provides a foundation for creating command operations that can leverage dependency injection to access necessary services and manage their state effectively in a CLI application. The constructor takes an IServiceProvider instance, a prefix for constructing qualified names, the name of the operation, an optional member type, and any aliases for the operation. The GetOperations method is overridden to locate and manage child operations based on the provided commands, allowing for a hierarchical structure of commands and sub-commands. The InvokeWhenContextIsOwned method can be overridden to define specific behavior when the context is owned by this operation, while the CanBypass and CanBypassAsync methods provide a mechanism to determine if the operation can bypass execution based on the remaining operations. This design promotes modularity, testability, and maintainability in a CLI application by enabling flexible command operations that can be easily extended and integrated with other components through dependency injection.
/// </summary>
/// <typeparam name="T">The type of the injectable command operation.</typeparam>
/// <param name="serviceProvider">The service provider for resolving dependencies.</param>
/// <param name="prefix">The prefix for constructing qualified names.</param>
/// <param name="name">The name of the operation.</param>
/// <param name="memberOfType">The type that the command operation is associated with.</param>
/// <param name="aliases">Any aliases for the operation.</param>
public abstract class InjectableCommandOperationBase<T>(IServiceProvider serviceProvider, string prefix, string name, Type? memberOfType, params string[] aliases)
    : CommandOperationBase(name, aliases), IInjectableCommandOperation
    where T : IInjectableCommandOperation
{
    private IInjectableCommandOperation[]? cachedOperations = null;
    private IInjectableCommandOperation[] remainingOperations = [];

    private IInjectableCommandOperation[] LocateOperations(IEnumerable<string> commands)
    {
        var serviceLocator = string.Empty;
        List<IInjectableCommandOperation> operations = [];
        foreach (var command in commands.Skip(1))
        {
            serviceLocator = string.IsNullOrWhiteSpace(serviceLocator) ?
                serviceLocator = $"{prefix}-{command}"
                : serviceLocator = $"{serviceLocator}-{command}";
            var service = Services.GetKeyedService<IInjectableCommandOperation>(serviceLocator);

            if (service is not null)
            {
                operations.Add(service);
            }
        }

        return [.. operations];
    }

    /// <summary>
    /// Defines a method to retrieve the operations that can be invoked based on a collection of command strings. This method is responsible for locating and managing child operations based on the provided commands, allowing for a hierarchical structure of commands and sub-commands in a CLI application. The implementation of this method should utilize the service provider to resolve the appropriate operations based on the command strings, and it should also manage the remaining operations that can be invoked after the current operation is executed. By overriding this method, you can customize how operations are retrieved and organized based on the command input, enhancing the flexibility and extensibility of the command handling system in the application.
    /// </summary>
    /// <param name="commands">The collection of command strings to evaluate for operation retrieval.</param>
    /// <returns>An enumerable of ICommandOperation instances that can be invoked based on the provided commands.</returns>
    protected override IEnumerable<ICommandOperation> GetOperations(IEnumerable<string> commands)
    {
        base.GetOperations(commands);
        var currentType = typeof(T);

        var operations = cachedOperations ??= LocateOperations(commands);

        remainingOperations = [.. operations.Skip(1)];

        foreach (var operation in operations)
        {
            operation.CachedOperations = remainingOperations;
            operation.ListOperations = ListOperations;
        }

        ICommandOperation[] ownedOperations = [.. operations.Where(x => x.MemberOfType == currentType)];

        AddSupportedOperations(ownedOperations);

        if (ListOperations)
        {
            var managedStream = serviceProvider.GetRequiredService<IManagedStream>();

            managedStream.Out.WriteLineAsync($"Operation: {Name}, FQN: {QualifiedName}", CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        return ownedOperations;
    }

    /// <summary>
    /// Invoked when the context is owned by this operation. This method can be overridden to define specific behavior when the context is owned by this operation, allowing for customized execution logic based on the command input and the current state of the operation. The implementation of this method should ensure that any necessary actions are performed when the context is owned, such as executing specific commands, managing state, or providing feedback to the user. By overriding this method, you can enhance the functionality and responsiveness of the command handling system in a CLI application by tailoring the behavior of operations based on their ownership of the context.
    /// </summary>
    /// <param name="command">The collection of command strings to evaluate for execution.</param>
    /// <param name="cancellationToken">The cancellation token to observe while executing the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        //TODO: we should put a warning that owned context does nothing in a logger instance
        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if the operation can bypass execution based on the remaining operations. This method provides a mechanism to evaluate whether the operation can skip its execution logic and allow the remaining operations to be invoked instead. The implementation of this method should consider the state of the remaining operations and any relevant conditions to determine if bypassing is appropriate. By overriding this method, you can customize the behavior of the command handling system in a CLI application, allowing for more dynamic and responsive command execution based on the context and state of the operations involved.
    /// </summary>
    /// <param name="command">The collection of command strings to evaluate for bypassing execution.</param>
    /// <param name="cancellationToken">The cancellation token to observe while evaluating bypass conditions.</param>
    /// <returns>True if the operation can bypass execution; otherwise, false.</returns>
    protected virtual bool CanBypass(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        return RemainingOperations.Any();
    }

    /// <summary>
    /// Determines asynchronously if the operation can bypass execution based on the remaining operations. This method provides an asynchronous mechanism to evaluate whether the operation can skip its execution logic and allow the remaining operations to be invoked instead. The implementation of this method should consider the state of the remaining operations and any relevant conditions to determine if bypassing is appropriate, while also supporting asynchronous operations that may be involved in the evaluation process. By overriding this method, you can customize the behavior of the command handling system in a CLI application, allowing for more dynamic and responsive command execution based on the context and state of the operations involved, while also accommodating asynchronous scenarios that may arise during command processing.
    /// </summary>
    /// <param name="command">The collection of command strings to evaluate for bypassing execution.</param>
    /// <param name="cancellationToken">The cancellation token to observe while evaluating bypass conditions.</param>
    /// <returns>A task representing the asynchronous operation, with a result of true if the operation can bypass execution; otherwise, false.</returns>
    protected virtual Task<bool> CanBypassAsync(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        return Task.FromResult(CanBypass(command, cancellationToken));
    }

    /// <summary>
    /// Invokes the operation with a given collection of command strings and a cancellation token. This method is responsible for executing the logic associated with the command operation based on the provided collection of command strings. The implementation of this method should ensure that the appropriate logic is executed based on the command input, while also considering any conditions for bypassing execution and managing the remaining operations. By overriding this method, you can customize the behavior of the command handling system in a CLI application, allowing for more dynamic and responsive command execution based on the context and state of the operations involved, while also accommodating asynchronous scenarios that may arise during command processing.
    /// </summary>
    /// <param name="command">The collection of command strings to be processed by the operation.</param>
    /// <param name="cancellationToken">The cancellation token to observe while executing the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnInvokeAsync(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        if (await CanBypassAsync(command, cancellationToken))
        {
            //TODO: we can refine this later with injected behaviours if we need commands needing to do sequential work!
            return;
        }

        await InvokeWhenContextIsOwned(command, cancellationToken);
    }

    /// <summary>
    /// Initialises the operation with an optional CommandOperationState parameter. This method is responsible for setting up the operation's state and preparing it for invocation. The CommandOperationState can be used to manage the current state of the operation, including any relevant data or context needed for executing commands. If no state is provided, the operation can initialize with a default state or remain uninitialized until invoked. This allows for a flexible initialization process that can accommodate various scenarios in a CLI application. By overriding this method, you can ensure that the operation is properly initialized and ready to handle command execution based on its specific requirements and context within the application.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve dependencies for the operation.</param>
    /// <param name="prefix">The prefix for the command operation, used to qualify the command name.</param>
    /// <param name="name">The name of the command operation.</param>
    /// <param name="aliases">Optional aliases for the command operation.</param>
    protected InjectableCommandOperationBase(IServiceProvider serviceProvider, string prefix, string name, params string[] aliases)
        : this(serviceProvider, prefix, name, null, aliases)
    {

    }

    /// <summary>
    /// Gets the collection of remaining operations that can be invoked after the current operation is executed. The RemainingOperations property provides access to the operations that are still available for invocation based on the command input and the structure of the command hierarchy. This allows for managing the flow of command execution and determining which operations can be invoked next in a CLI application. By accessing this property, you can implement logic to handle the remaining operations, such as providing feedback to the user, managing state transitions, or implementing conditional execution based on the available operations. This design enhances the flexibility and responsiveness of the command handling system in the application by allowing for dynamic management of operations based on user input and command structure.
    /// </summary>
    protected IEnumerable<IInjectableCommandOperation> RemainingOperations => remainingOperations;

    /// <summary>
    /// Gets the qualified name of the command operation, which serves as a unique identifier for the operation within the application. The QualifiedName property provides a string representation of the operation's identity, which can be used for various purposes such as logging, debugging, or routing commands to the appropriate operations based on their qualified names. This property can help distinguish between different operations that may have similar functionality but belong to different contexts or components in a CLI application. By utilizing the qualified name, you can enhance the clarity and maintainability of your command handling system by providing a clear and consistent way to identify and reference command operations throughout the application.
    /// </summary>
    public string QualifiedName => $"{prefix}-{Name}";

    /// <summary>
    /// Gets the service provider for resolving dependencies within the command operation. The Services property provides access to the IServiceProvider instance that can be used to resolve services and dependencies required by the command operation. This allows for leveraging dependency injection to manage the lifecycle and dependencies of the operation, promoting modularity and testability in a CLI application. By accessing the service provider, you can retrieve necessary services, such as logging, configuration, or other application-specific services, to enhance the functionality and behavior of the command operation. This design enables a more flexible and maintainable architecture by decoupling the command operation from its dependencies and allowing for easier integration with other components in the application.
    /// </summary>
    public IServiceProvider Services => serviceProvider;

    /// <summary>
    /// Gets the member type that the command operation is associated with. The MemberOfType property provides information about the type that the command operation belongs to, which can be useful for identifying the context and scope of the operation within the application. This property can be used to determine the class or component that the command operation is a part of, allowing for better organization and management of operations in a CLI application. By accessing the member type, you can also leverage reflection or other techniques to dynamically discover and invoke operations based on their associated types, enhancing the flexibility and extensibility of the command handling system in the application.
    /// </summary>
    public Type? MemberOfType => memberOfType;

    /// <summary>
    /// Gets or sets a collection of cached operations that can be stored and retrieved for later use. The CachedOperations property allows for managing previously executed operations, enabling features such as command history, undo/redo functionality, or caching results for performance optimization in a CLI application. By setting this property, you can provide a collection of IInjectableCommandOperation instances that represent the cached operations, which can be accessed and utilized as needed during the execution of commands. This promotes a more dynamic and responsive user experience by allowing users to interact with their command history or access frequently used operations without having to re-enter commands manually.
    /// </summary>
    public IEnumerable<IInjectableCommandOperation> CachedOperations { set => cachedOperations = [.. value]; }
}
