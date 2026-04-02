using IDFCR.Abstractions.Cli.StateManagement;

namespace IDFCR.Abstractions.Cli.Operations;

/// <summary>
/// Represents a base implementation of the ICommandOperation interface, providing common functionality for command operations in a CLI application. This class allows for defining a command with a name and optional aliases, managing supported sub-operations, handling command invocation, and maintaining the state of the operation. It also provides mechanisms for initializing the operation and its sub-operations with a shared state. Derived classes can override methods to customize token parsing, operation retrieval, and invocation behavior as needed.
/// </summary>
/// <param name="name">The name of the command.</param>
/// <param name="aliases">Optional aliases for the command.</param>
public abstract class CommandOperationBase(string name, params string[] aliases) : ICommandOperation
{
    IEnumerable<ICommandOperation> ICommandOperation.SupportedOperations => _operations;

    private readonly List<ICommandOperation> _operations = [];
    private CommandOperationState _state = new();
    private bool _isInitialised = false;

    /// <summary>
    /// Gets the parameters for the command operation. This property is initialized lazily when the GetOperations method is called, using an ArgumentSplitter to parse the commands into parameters. The Parameters property is a dictionary that maps parameter names to their corresponding Parameter objects, which can contain information about the parameter such as its value, type, and whether it is required. This allows for flexible handling of command parameters in derived classes when implementing the OnInvokeAsync method.
    /// </summary>
    protected internal virtual IDictionary<string, Parameter>? Parameters { get; private set; }

    /// <summary>
    /// Gets the tokens from the input command string. By default, this method splits the command string by spaces, removing empty entries and trimming whitespace. Derived classes can override this method to implement custom tokenization logic if needed, such as handling quoted strings or different delimiters. The resulting tokens are used in the InvokeAsync method to determine which operations to invoke based on the command input.
    /// </summary>
    /// <param name="command">The input command string.</param>
    /// <returns>An enumerable of tokens extracted from the command string.</returns>
    protected virtual IEnumerable<string> GetTokens(string command)
    {
        return command.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    /// <summary>
    /// Gets the supported operations for the command based on the input commands. By default, this method initializes the Parameters property using an ArgumentSplitter to parse the input commands into parameters. It then returns the list of supported sub-operations. Derived classes can override this method to implement custom logic for determining which operations are supported based on the input commands, such as filtering operations or modifying parameters before returning the supported operations. The resulting operations are used in the InvokeAsync method to determine which sub-operations to invoke based on the command input.
    /// </summary>
    /// <param name="commands">The input commands.</param>
    /// <returns>An enumerable of supported command operations.</returns>
    protected virtual IEnumerable<ICommandOperation> GetOperations(IEnumerable<string> commands)
    {
        Parameters ??= ArgumentSplitter.Split(commands);
        return _operations;
    }

    /// <summary>
    /// Gets invoked when the command operation is executed. This method is called by the InvokeAsync method after determining that the command matches the operation's name or aliases. The OnInvokeAsync method receives the remaining tokens after the command name as input, allowing derived classes to implement the specific logic for handling the command execution based on the provided parameters and tokens. This is where the core functionality of the command operation should be implemented in derived classes.
    /// </summary>
    /// <param name="command">The input command string.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected abstract Task OnInvokeAsync(IEnumerable<string> command, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the name of the command operation. This property is initialized through the constructor and is used to identify the command when invoking operations. The Name property is compared against the input command tokens in the InvokeAsync method to determine if the operation should be executed. Derived classes can override this property if needed, but it is typically set through the constructor and remains constant for a given command operation instance.
    /// </summary>
    public string Name => name;

    /// <summary>
    /// Gets the aliases for the command operation. This property is initialized through the constructor and provides alternative names that can be used to invoke the command operation. The Aliases property is compared against the input command tokens in the InvokeAsync method to determine if the operation should be executed, allowing for flexibility in how commands can be invoked. Derived classes can override this property if needed, but it is typically set through the constructor and remains constant for a given command operation instance.
    /// </summary>
    public IEnumerable<string> Aliases => aliases;

    /// <summary>
    /// Gets or sets a value indicating whether the command operation should be included in the list of operations when listing available commands. This property can be used to control whether the command operation is visible to users when they request a list of available commands, allowing for certain operations to be hidden from the user interface while still being accessible for invocation if the user knows the command name or alias. By default, this property is false, meaning that the command operation will not be listed unless explicitly set to true.
    /// </summary>
    public bool ListOperations { protected get; set; }

    /// <summary>
    /// Gets or sets the state of the command operation. The State property is used to maintain shared state information across the command operation and its sub-operations. This state can be initialized during the Initialise method and can be accessed by derived classes when implementing the OnInvokeAsync method to manage stateful information related to the command execution. The State property can only be set once during initialization, and any subsequent attempts to set it will result in an InvalidOperationException being thrown, ensuring that the state remains consistent throughout the lifecycle of the command operation.
    /// </summary>
    public CommandOperationState State { 
        get => _state; 
        set { 
            if (!_isInitialised)
            {
                _state = value;
                _isInitialised = true;
            }
            else
            {
                throw new InvalidOperationException("Cannot set State after initialisation.");
            }
        } 
    }

    /// <summary>
    /// Determines whether the specified name matches the command operation's name or any of its aliases. This method is used in the InvokeAsync method to check if the input command token corresponds to this command operation, allowing for flexible invocation using either the primary name or any of the defined aliases. The comparison is case-insensitive, ensuring that users can invoke commands without worrying about capitalization. Derived classes can override this method if they need to implement custom logic for matching command names or aliases.
    /// </summary>
    /// <param name="name">The name to compare with the command operation's name and aliases.</param>
    /// <returns>True if the specified name matches the command operation's name or any of its aliases; otherwise, false.</returns>
    public bool Equals(string? name)
    {
        return Name
            .Equals(name, StringComparison.OrdinalIgnoreCase)
            || Aliases.Contains(name, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current command operation. This method checks if the provided object is a CommandOperationBase instance and then compares its name and aliases with the current instance using case-insensitive comparisons. This allows for determining equality based on the command's identity rather than reference equality, enabling scenarios where different instances of CommandOperationBase with the same name and aliases are considered equal. Derived classes can override this method if they need to implement custom logic for determining equality, but the default implementation provides a reasonable basis for comparing command operations based on their names and aliases.
    /// </summary>
    /// <param name="obj">The object to compare with the current command operation.</param>
    /// <returns>True if the specified object is equal to the current command operation; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is CommandOperationBase other &&
               Equals(other.Name, StringComparison.OrdinalIgnoreCase) &&
               Aliases.SequenceEqual(other.Aliases, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Defines a hash code for the command operation based on its name and aliases. This method combines the hash codes of the Name and Aliases properties to generate a unique hash code for the command operation, allowing it to be used effectively in hash-based collections such as dictionaries or hash sets. The GetHashCode implementation ensures that command operations with the same name and aliases will produce the same hash code, which is consistent with the Equals method's logic for determining equality. Derived classes can override this method if they need to implement custom logic for generating hash codes, but the default implementation provides a reasonable basis for hashing command operations based on their identity.
    /// </summary>
    /// <returns>A hash code for the current command operation.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Aliases);
    }

    /// <summary>
    /// Defines the logic for invoking the command operation based on the input commands. This method first retrieves the supported operations using the GetOperations method, then checks if the first command token matches the command operation's name or aliases. If it does, it invokes the OnInvokeAsync method with the remaining tokens. If there are additional tokens, it looks for a matching sub-operation among the supported operations and invokes it recursively with the remaining tokens. This allows for a hierarchical structure of commands and sub-commands, enabling complex command handling in a CLI application. Derived classes can override this method to implement custom invocation logic if needed, but the default implementation provides a reasonable basis for handling command invocation based on token matching and operation retrieval.
    /// </summary>
    /// <param name="commands">The sequence of command tokens to process.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task InvokeAsync(IEnumerable<string> commands, CancellationToken cancellationToken)
    {
        var operations = GetOperations(commands);
        if (Equals(commands.FirstOrDefault()))
        {
            await OnInvokeAsync(commands.Skip(1), cancellationToken);
        }
        else
        {
            //fix for the wrong root does nothing
            return;
        }

        if (commands.Count() > 1)
        {
            var currentOperation = operations
                .FirstOrDefault(x => x.Equals(commands.ElementAtOrDefault(1)));

            if (currentOperation is null)
            {
                return;
            }

            var nextElements = commands.Skip(1);

            await currentOperation.InvokeAsync(nextElements, cancellationToken);
        }
    }

    /// <summary>
    /// Invokes the command operation based on the input command string. This method is a convenience overload that takes a single command string, tokenizes it using the GetTokens method, and then calls the main InvokeAsync method with the resulting tokens. This allows for easier invocation of commands when you have a raw command string, without needing to manually split it into tokens before calling InvokeAsync. Derived classes can override this method if they need to implement custom logic for handling raw command strings, but the default implementation provides a reasonable basis for invoking commands based on tokenized input.
    /// </summary>
    /// <param name="command">The raw command string to process.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task InvokeAsync(string command, CancellationToken cancellationToken)
    {
        return InvokeAsync(GetTokens(command), cancellationToken);
    }

    /// <summary>
    /// Adds multiple supported operations to the command operation. This method takes a variable number of ICommandOperation instances and adds them to the list of supported operations for this command. This allows for easily defining sub-commands or related operations that can be invoked under this command, enabling a hierarchical structure of commands in a CLI application. Derived classes can use this method to add their specific sub-operations during initialization or construction, ensuring that the command operation has the necessary operations available for invocation when processing commands.
    /// </summary>
    /// <param name="operations">The operations to add to the command operation.</param>
    public void AddSupportedOperations(params ICommandOperation[] operations)
    {
        foreach (var operation in operations)
        {
            AddSupportedOperation(operation);
        }
    }

    /// <summary>
    /// Adds a single supported operation to the command operation. This method takes an ICommandOperation instance and adds it to the list of supported operations for this command. This allows for incrementally building up the set of sub-commands or related operations that can be invoked under this command, enabling a hierarchical structure of commands in a CLI application. Derived classes can use this method to add specific sub-operations during initialization or construction, ensuring that the command operation has the necessary operations available for invocation when processing commands.
    /// </summary>
    /// <param name="operation">The operation to add to the command operation.</param>
    public void AddSupportedOperation(ICommandOperation operation)
    {
        _operations.Add(operation);
    }


    /// <summary>
    /// Initialises the command operation and its supported sub-operations with the provided state. This method sets the state for the command operation and then calls the Initialise method on each of the supported operations, passing the same state to ensure that all operations share the same state information. This allows for consistent state management across the command operation and its sub-operations, enabling them to access and modify shared state as needed during command execution. The Initialise method can only be called once, and any subsequent attempts to call it will result in an InvalidOperationException being thrown, ensuring that the initialization process is performed only once and that the state remains consistent throughout the lifecycle of the command operation.
    /// </summary>
    /// <param name="state">The state to initialise the command operation with.</param>
    /// <exception cref="InvalidOperationException">Thrown if the command operation has already been initialised.</exception>
    public void Initialise(CommandOperationState? state = null)
    {
        if (_isInitialised)
            throw new InvalidOperationException("Already initialised.");

        if (state is not null)
        {
            _state = state;
        }

        foreach (var operation in _operations)
        {
            operation.Initialise(_state);
        }

        _isInitialised = true;
    }
}