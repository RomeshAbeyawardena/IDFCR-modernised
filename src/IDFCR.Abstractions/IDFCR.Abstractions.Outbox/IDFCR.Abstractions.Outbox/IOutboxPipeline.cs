
namespace IDFCR.Abstractions.Outbox;

/// <summary>
/// Represents a pipeline for processing outbox messages asynchronously. This interface defines methods for starting and stopping the outbox processing loop, allowing for graceful handling of cancellation requests and resource cleanup.
/// </summary>
public interface IOutboxPipeline : IAsyncDisposable
{
    /// <summary>
    /// Starts the outbox processing loop in a background task. If the loop is already running, a warning is logged and no action is taken.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task StartAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Stops the outbox processing loop. If the loop is not running, a warning is logged and no action is taken.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task StopAsync(CancellationToken cancellationToken = default);
}
