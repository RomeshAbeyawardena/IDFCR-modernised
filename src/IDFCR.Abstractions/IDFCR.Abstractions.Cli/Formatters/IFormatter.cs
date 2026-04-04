namespace IDFCR.Abstractions.Cli.Formatters;

/// <summary>
/// Represents a formatter that can format objects into a string representation and write the formatted output to a managed stream. The IFormatter interface defines methods for flushing the accumulated formatted output, formatting values asynchronously, and managing resources through synchronous and asynchronous disposal. Implementations of this interface can be used to create various types of formatters that direct their output to either the error stream or the standard output stream of a managed stream, allowing for flexible handling of formatted messages in different contexts.
/// </summary>
public interface IFormatter : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Flushes the accumulated formatted output to the appropriate stream based on the specified target. This method blocks until the flush is complete and is safe to call concurrently — subsequent calls will wait for any in-progress flush to finish first.
    /// </summary>
    void Flush();

    /// <summary>
    /// Flushes the accumulated formatted output to the appropriate stream based on the specified target. This method uses a semaphore to prevent concurrent flushes, writes the contents of the StringBuilder to the target stream, and resets the StringBuilder afterwards. The operation can be cancelled using the provided CancellationToken.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous flush operation.</returns>
    Task FlushAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Formats the specified value asynchronously and writes the formatted output to the managed stream.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous format operation.</returns>
    Task FormatAsync(object value, CancellationToken cancellationToken);
    /// <summary>
    /// Formats the specified value of type T asynchronously and writes the formatted output to the managed stream. This method allows for type-specific formatting logic to be implemented in derived classes, enabling more efficient formatting for certain types without the need for boxing or type checks that may occur in the non-generic FormatAsync method.
    /// </summary>
    /// <typeparam name="T">The type of the value to format.</typeparam>
    /// <param name="value">The value to format.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous format operation.</returns>
    Task FormatAsync<T>(T value, CancellationToken cancellationToken);
}
