namespace IDFCR.Abstractions.Cli.Extensions;

/// <summary>
/// Represents an exception that is thrown when a specified key is not found in a collection or data structure. This exception can be used to indicate that an attempt was made to access a key that does not exist, providing a clear and specific error message to help developers identify and resolve the issue. By using this custom exception, developers can improve error handling and debugging in applications and systems that involve key-based data access, ensuring that missing keys are properly reported and addressed.
/// </summary>
/// <param name="messsage">The error message that explains the reason for the exception.</param>
/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
public sealed class KeyNotFoundException(string messsage, Exception? innerException) : Exception(messsage, innerException)
{
    /// <summary>
    /// Initialises a new instance of the KeyNotFoundException class with a specified key that was not found. If the key is null, empty, or consists only of whitespace characters, a default error message "Key not found" is used. Otherwise, the error message includes the specific key that was not found, providing more context for debugging and error handling purposes. By using this constructor, developers can easily create instances of KeyNotFoundException with informative messages when a key is missing from a collection or data structure within applications and systems that involve key-based data access.
    /// </summary>
    /// <param name="key">The key that was not found.</param>
    public KeyNotFoundException(string? key = null) : this(string.IsNullOrWhiteSpace(key) 
        ? "Key not found" : $"Key '{key}' not found", null) { }
}
