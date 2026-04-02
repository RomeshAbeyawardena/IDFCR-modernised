namespace IDFCR.Abstractions.Cli.Operations;

/// <summary>
/// Represents an injectable command operation root that extends the IInjectableCommandOperation interface. This interface is designed to serve as the root of a command operation hierarchy, allowing for the execution of commands based on their names. The CanExecute method is defined to determine if a given command string can be executed by this command operation root, providing a way to validate and route commands to the appropriate operations in a CLI application. By implementing this interface, you can create a command operation root that can handle various commands and delegate them to the appropriate child operations based on their names, enabling a flexible and extensible command handling system in the application.
/// </summary>
public interface IInjectableCommandOperationRoot : IInjectableCommandOperation
{
    /// <summary>
    /// Determines if a given command string can be executed by this command operation root. The CanExecute method takes a string parameter representing the command to be executed and returns a boolean value indicating whether the command is valid and can be processed by this command operation root. This method can be used to validate user input, route commands to the appropriate child operations, or provide feedback to the user about available commands in a CLI application. By implementing this method, you can ensure that only valid commands are executed and that users receive appropriate guidance when interacting with the command handling system in the application.
    /// </summary>
    /// <param name="command">The command string to be evaluated for execution.</param>
    /// <returns>True if the command can be executed; otherwise, false.</returns>
    bool CanExecute(string command);
}
