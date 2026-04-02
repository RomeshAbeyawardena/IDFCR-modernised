namespace IDFCR.Abstractions.Cli.Operations;

/// <summary>
/// Represents a command operation that can be executed in a command-line interface (CLI) context. An operation is defined by its name and a set of aliases that can be used to invoke it. This interface serves as a contract for implementing various CLI operations, allowing for consistent handling of commands and their associated logic across different implementations.
/// </summary>
public interface IOperation
{
    /// <summary>
    /// Gets the name of the operation, which is used to identify and invoke the operation in a CLI context. The name should be unique among all operations to avoid conflicts when executing commands. It is typically a single word or a short phrase that describes the action performed by the operation. For example, an operation that lists files might have the name "list" or "ls".
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Gets a collection of aliases for the operation, which are alternative names that can be used to invoke the same operation in a CLI context. Aliases provide flexibility and convenience for users, allowing them to use different terms or abbreviations to execute the same command. For example, an operation with the name "list" might have aliases like "ls" or "dir". The collection of aliases should be unique among all operations to prevent ambiguity when executing commands.
    /// </summary>
    IEnumerable<string> Aliases { get; }
}
