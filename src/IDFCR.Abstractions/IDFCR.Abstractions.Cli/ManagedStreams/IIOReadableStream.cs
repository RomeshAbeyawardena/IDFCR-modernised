namespace IDFCR.Abstractions.Cli.ManagedStreams;

/// <summary>
/// Represents a contract for a readable stream that provides methods for reading characters and lines of text. This interface defines the necessary functionality for reading from a stream, allowing for the implementation of various types of readable streams that can be used in different contexts, such as console input, file input, or network streams. The ReadChar method allows for reading individual characters from the stream, while the ReadLineAsync method enables asynchronous reading of lines of text, supporting cancellation through a CancellationToken. Implementations of this interface can provide specific behavior for how characters and lines are read based on the underlying stream type and requirements of the application.
/// </summary>
public interface IIOReadableStream
{
    /// <summary>
    /// Defines a method for reading a single character from the stream. This method is intended to be implemented by classes that provide specific behavior for reading characters based on the underlying stream type. The ReadChar method allows for reading individual characters, which can be useful for scenarios where character-by-character processing is required, such as interactive console input or parsing of text streams. The implementation of this method should handle the logic for retrieving the next character from the stream and returning it to the caller, while also considering any necessary error handling or end-of-stream conditions.
    /// </summary>
    /// <returns>The next character from the stream, or char.MinValue if no character is available.</returns>
    char ReadChar();
    /// <summary>
    /// Defines a method for asynchronously reading a line of text from the stream. This method is intended to be implemented by classes that provide specific behavior for reading lines based on the underlying stream type. The ReadLineAsync method allows for reading entire lines of text, which can be useful for scenarios where line-by-line processing is required, such as reading user input from the console or processing text files. The asynchronous nature of this method enables non-blocking execution, allowing the application to remain responsive while waiting for input. The method also accepts a CancellationToken to support cooperative cancellation, allowing the caller to cancel the read operation if needed. The implementation of this method should handle the logic for retrieving the next line of text from the stream and returning it to the caller, while also considering any necessary error handling or end-of-stream conditions.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string?> ReadLineAsync(CancellationToken cancellationToken);
    
}
