using IDFCR.Abstractions.Cli.ManagedStreams;
using System.Text;

namespace IDFCR.Abstractions.Cli.Formatters;

/// <summary>
/// Represents a base class for formatters that write formatted output to a managed stream. It provides common functionality for managing a StringBuilder to accumulate formatted output and defines methods for flushing the accumulated output to the appropriate stream (either error or output) based on the specified target. The class also implements the IFormatter interface, allowing derived classes to focus on implementing the specific formatting logic while handling the management of the output stream and disposal of resources.
/// </summary>
/// <param name="managedStream"></param>
public abstract class ManagedStreamFormatterBase(IManagedStream managedStream) : IFormatter
{
    private StringBuilder _stringBuilder = new();
    private readonly SemaphoreSlim _flushLock = new(1, 1);
    private bool _disposed;

    /// <summary>
    /// Gets or sets the target stream for the formatter. This property determines whether the formatted output will be written to the error stream or the output stream of the managed stream. The formatter will use this target when flushing the accumulated formatted output, allowing for flexible routing of messages based on their nature (e.g., errors vs. regular output).
    /// </summary>
    public ManagedStreamTarget Target { get; set; }

    /// <summary>
    /// Gets the <see cref="StringBuilder"/> used to accumulate formatted output. Derived classes should append their formatted content here inside <see cref="FormatAsync(object, CancellationToken)"/> and <see cref="FormatAsync{T}(T, CancellationToken)"/> implementations.
    /// </summary>
    protected StringBuilder StringBuilder => _stringBuilder;

    private async Task FlushCoreAsync(CancellationToken cancellationToken)
    {
        await _flushLock.WaitAsync(cancellationToken);
        try
        {
            if (Target == ManagedStreamTarget.Error)
                await managedStream.Error.WriteAsync(b => b.Append(_stringBuilder), cancellationToken);
            else
                await managedStream.Out.WriteAsync(b => b.Append(_stringBuilder), cancellationToken);

            _stringBuilder = new();
        }
        finally
        {
            _flushLock.Release();
        }
    }

    private async ValueTask DisposeAsyncCore()
    {
        await FlushCoreAsync(CancellationToken.None);
        _stringBuilder.Clear();
        _flushLock.Dispose();
    }

    /// <summary>
    /// Defines a method to release resources used by the formatter synchronously. Flushes any remaining buffered output before disposing. Derived classes can override this method to add their own disposal logic.
    /// </summary>
    public virtual void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        GC.SuppressFinalize(this);
        DisposeAsyncCore().AsTask().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Defines an asynchronous method to release resources used by the formatter. Flushes any remaining buffered output before disposing. Derived classes can override this method to add their own asynchronous disposal logic.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous disposal operation.</returns>
    public virtual async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        GC.SuppressFinalize(this);
        await DisposeAsyncCore();
    }

    /// <summary>
    /// Defines a method to flush the accumulated formatted output to the appropriate stream based on the specified target. This method blocks until the flush is complete and is safe to call concurrently — subsequent calls will wait for any in-progress flush to finish first.
    /// </summary>
    public virtual void Flush() =>
        FlushCoreAsync(CancellationToken.None).GetAwaiter().GetResult();

    /// <summary>
    /// Defines an asynchronous method to flush the accumulated formatted output to the appropriate stream based on the specified target. This method uses a semaphore to prevent concurrent flushes, writes the contents of the StringBuilder to the target stream, and resets the StringBuilder afterwards.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the process.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous flush operation.</returns>
    public Task FlushAsync(CancellationToken cancellationToken) =>
        FlushCoreAsync(cancellationToken);

    /// <summary>
    /// Defines an asynchronous method to format a given value and accumulate the formatted output in the <see cref="StringBuilder"/>. Derived classes must implement their own specific formatting logic and append the result to <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the process.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous formatting operation.</returns>
    public virtual Task FormatAsync(object value, CancellationToken cancellationToken)
    {
        return FormatAsync<object>(value, cancellationToken);
    }

    /// <summary>
    /// Defines an asynchronous method to format a given value of a specific type and accumulate the formatted output in the <see cref="StringBuilder"/>. Derived classes must implement their own specific formatting logic and append the result to <see cref="StringBuilder"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value to format.</typeparam>
    /// <param name="value">The value to format.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the process.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous formatting operation.</returns>
    public abstract Task FormatAsync<T>(T value, CancellationToken cancellationToken);
}
