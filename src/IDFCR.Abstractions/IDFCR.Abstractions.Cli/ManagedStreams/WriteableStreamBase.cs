using System.Text;

namespace IDFCR.Abstractions.Cli.ManagedStreams;

/// <summary>
/// Represents a writeable stream that wraps a TextWriter, allowing it to be used as an IIOWriteableStream in scenarios where output needs to be written to a stream. This class inherits from WriteableStreamBase, which provides the necessary implementation to write to the underlying TextWriter and expose it through the IIOWriteableStream interface. It is designed to facilitate writing output in a structured manner, enabling developers to easily direct output to various destinations such as console, files, or other text-based streams without needing to manage the underlying TextWriter directly.
/// </summary>
/// <param name="stream">The TextWriter to wrap.</param>
public class WriteableStreamBase(TextWriter stream) : IIOWriteableStream
{
    /// <summary>
    /// Writes the specified string to the underlying TextWriter asynchronously. This method accepts a string value and a CancellationToken, allowing for non-blocking write operations. The method writes the provided string to the stream and supports cancellation of the write operation if needed. Derived classes can use this method to write output to the stream in a structured manner, while handling any necessary formatting or additional logic before writing the final output.
    /// </summary>
    /// <param name="builderAction">An action that builds the string to be written.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public Task WriteAsync(Action<StringBuilder> builderAction, CancellationToken cancellationToken)
    {
        var builder = new StringBuilder();
        builderAction(builder);
        return stream.WriteAsync(builder, cancellationToken);
    }
}
