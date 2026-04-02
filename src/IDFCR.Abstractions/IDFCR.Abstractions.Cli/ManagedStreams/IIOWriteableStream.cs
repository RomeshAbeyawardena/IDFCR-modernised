using System.Text;

namespace IDFCR.Abstractions.Cli.ManagedStreams;

/// <summary>
/// Represents a contract for a writeable stream that provides a method for writing text to the stream. This interface defines the necessary functionality for writing to a stream, allowing for the implementation of various types of writeable streams that can be used in different contexts, such as console output, file output, or network streams. The WriteAsync method allows for asynchronous writing of text to the stream, supporting cancellation through a CancellationToken. Implementations of this interface can provide specific behavior for how text is written based on the underlying stream type and requirements of the application. The method accepts an <see cref="Action{StringBuilder}"/> delegate, which allows the caller to build the text to be written using a StringBuilder, providing flexibility in constructing the output before it is sent to the stream. The implementation of this method should handle the logic for writing the constructed text to the stream while also considering any necessary error handling or resource management.
/// </summary>
public interface IIOWriteableStream
{
    /// <summary>
    /// Defines a method for asynchronously writing text to the stream. This method is intended to be implemented by classes that provide specific behavior for writing text based on the underlying stream type. The WriteAsync method allows for writing text to the stream using an <see cref="Action{StringBuilder}"/> delegate, which enables the caller to construct the text to be written using a StringBuilder. The asynchronous nature of this method allows for non-blocking execution, enabling the application to remain responsive while performing write operations. The method also accepts a CancellationToken to support cooperative cancellation, allowing the caller to cancel the write operation if needed. The implementation of this method should handle the logic for writing the constructed text to the stream while also considering any necessary error handling or resource management.
    /// </summary>
    /// <param name="builder">An action that constructs the text to be written using a StringBuilder.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    Task WriteAsync(Action<StringBuilder> builder, CancellationToken cancellationToken);
}
