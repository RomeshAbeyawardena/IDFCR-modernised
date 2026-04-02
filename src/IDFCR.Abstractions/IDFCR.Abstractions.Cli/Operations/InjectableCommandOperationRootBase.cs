namespace IDFCR.Abstractions.Cli.Operations;

/// <summary>
/// Represents the root of an injectable command operation tree. This is the entry point for command execution and can contain child operations that represent subcommands or options. The root operation is responsible for determining if a given command matches its own name or any of its aliases, and if so, it can execute the associated logic or delegate to child operations as needed.
/// </summary>
/// <typeparam name="T">The type of the injectable command operation root.</typeparam>
/// <param name="serviceProvider">The service provider for resolving dependencies.</param>
/// <param name="prefix">The prefix for constructing qualified names.</param>
/// <param name="name">The name of the operation.</param>
/// <param name="memberOfType"></param>
/// <param name="aliases"></param>
public abstract class InjectableCommandOperationRootBase<T>(IServiceProvider serviceProvider, string prefix, string name, Type? memberOfType, params string[] aliases)
    : InjectableCommandOperationBase<T>(serviceProvider, prefix, name, memberOfType, aliases),
    IInjectableCommandOperationRoot
    where T : IInjectableCommandOperation
{
    /// <summary>
    /// Determines whether the specified command can be executed by this operation. This method checks if the command matches the name or any of the aliases of this operation. If it does, it returns true, indicating that this operation can handle the command. Otherwise, it returns false, allowing other operations to be considered for execution.
    /// </summary>
    /// <param name="command">The command string to evaluate for execution.</param>
    /// <returns>True if the operation can execute the command; otherwise, false.</returns>
    public virtual bool CanExecute(string command)
    {
        return Equals(GetTokens(command).FirstOrDefault());
    }
}
