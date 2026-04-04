namespace IDFCR.Abstractions.Cli.ManagedStreams;

/// <summary>
/// Represents a base class for readable streams that utilize a TextReader to read input. This class provides common functionality for reading characters and lines from the underlying TextReader, allowing derived classes to focus on implementing specific reading logic while leveraging the provided methods for character and line reading. The ReadableStreamBase class implements the IIOReadableStream interface, ensuring that it adheres to the expected contract for readable streams in the context of managed stream operations.
/// </summary>
/// <param name="reader"></param>
public abstract class ReadableStreamBase(TextReader reader) : IIOReadableStream
{
    /// <summary>
    /// Reads a single character from the underlying TextReader. This method returns the next character as an integer, or -1 if no more characters are available. The returned integer can be cast to a char to obtain the actual character value. Derived classes can use this method to read characters from the stream as needed, while handling end-of-stream conditions appropriately.
    /// </summary>
    /// <returns></returns>
    public virtual char ReadChar()
    {
        return (char)reader.Read();
    }

    /// <summary>
    /// Reads a line of text from the underlying TextReader asynchronously. This method returns the next line of text as a string, or null if no more lines are available. The asynchronous nature of this method allows for non-blocking reading operations, enabling derived classes to read lines from the stream without blocking the calling thread. The method also accepts a CancellationToken to support cancellation of the read operation if needed.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string?> ReadLineAsync(CancellationToken cancellationToken)
    {
        return await reader.ReadLineAsync(cancellationToken);
    }
}
