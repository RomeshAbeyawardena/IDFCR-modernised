using IDFCR.Abstractions.Outbox;
using IDFCR.Abstractions.Results;
using IDFCR.Utilities.Extensions;
using Microsoft.Extensions.Logging;

namespace IDFCR.Outbox.Extensions.Dispatchers;

/// <inheritdoc cref="IOutboxPipeline"/>
public abstract class OutboxPipelineBase<TMessage, TPagedQuery>(
    ILogger logger,
    IOutboxReaderFactory<TMessage> outboxReaderFactory,
    IOutboxDispatcher<TMessage, TPagedQuery> outboxDispatcher,
    int delay = 1000, int pageSize = 20) : IOutboxPipeline
    where TMessage : IOutboxEntity
    where TPagedQuery : IPagedQuery, new()
{
    private readonly CancellationTokenSource _cts = new();
    private CancellationTokenSource? _linkedCts;
    private Task? _loopTask;

    // Cached once to keep high-frequency loops allocation-lean
    private readonly string _messageTypeName = typeof(TMessage).Name;
    private async Task LoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogMethod(LogLevel.Information, "Background outbox processing loop started for type {MessageType}.", args: [_messageTypeName]);

            while (!cancellationToken.IsCancellationRequested)
            {
                await ExecuteAsync(cancellationToken);
                await Task.Delay(delay, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogMethod(LogLevel.Information, "Outbox loop for {MessageType} gracefully acknowledging internal cancellation request.", args: [_messageTypeName]);
        }
        catch (Exception ex)
        {
            logger.LogMethod(LogLevel.Critical, "A critical unhandled failure terminated the outbox processing loop for {MessageType}! Error details: {ErrorMessage}", args: [_messageTypeName, ex.Message]);
            throw;
        }
    }

    /// <summary>
    /// Sets the filters for the outbox query, including pagination parameters such as page index and page size. This method can be overridden in derived classes to customize the filtering logic as needed.
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    protected virtual TPagedQuery SetFilters(int pageIndex, int pageSize)
    {
        var query = new TPagedQuery()
        {
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        return query;
    }

    /// <summary>
    /// Executes a single processing cycle for the outbox pipeline, iterating through all compatible readers and dispatching messages in batches.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var readers = outboxReaderFactory.GetCompatibleReaders<TPagedQuery>();

        logger.LogMethod(LogLevel.Debug, "Executing processing cycle for outbox entity type {MessageType} across {ReaderCount} compatible readers.", args: [_messageTypeName, readers.Count()]);

        foreach (var reader in readers)
        {
            int currentPage = 0;
            bool hasMorePages = true;
            string readerName = reader.GetType().Name;

            while (hasMorePages && !cancellationToken.IsCancellationRequested)
            {
                logger.LogMethod(LogLevel.Debug, "Fetching page {PageIndex} (Size: {PageSize}) for {MessageType} via {ReaderName}.", args: [currentPage, pageSize, _messageTypeName, readerName]);

                var query = SetFilters(currentPage, pageSize);
                var messages = await reader.GetMessagesAsync(query, cancellationToken);

                if (messages is null || !messages.HasValue)
                {
                    logger.LogMethod(LogLevel.Warning, "Reader {ReaderName} returned an empty or invalid result set for page {PageIndex} of {MessageType}.", args: [readerName, currentPage, _messageTypeName]);
                    break;
                }

                int currentBatchCount = messages.Result?.Count() ?? 0;

                if (currentBatchCount > 0)
                {
                    logger.LogMethod(LogLevel.Information, "Pushing {MessageCount} outbox messages of type {MessageType} to the dispatcher.", args: [currentBatchCount, _messageTypeName]);

                    await outboxDispatcher.PushAsync(messages, cancellationToken);
                }
                else
                {
                    logger.LogMethod(LogLevel.Debug, "No pending messages found on page {PageIndex} for outbox type {MessageType}.", args: [currentPage, _messageTypeName]);
                }

                if (currentBatchCount < pageSize)
                {
                    hasMorePages = false;
                }
                else
                {
                    currentPage++;
                }
            }
        }
    }

    /// <summary>
    /// Starts the outbox processing loop in a background task. If the loop is already running, a warning is logged and no action is taken.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_loopTask is { IsCompleted: false })
        {
            logger.LogMethod(LogLevel.Warning, "Attempted to start the outbox pipeline for {MessageType}, but the background worker task is already active.", args: [_messageTypeName]);
            return Task.CompletedTask;
        }

        logger.LogMethod(LogLevel.Information, "Initialising startup routine for outbox pipeline: {MessageType}.", args: [_messageTypeName]);

        _linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken);
        _loopTask = Task.Run(() => LoopAsync(_linkedCts.Token), CancellationToken.None);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Signals the outbox processing loop to stop gracefully. If the loop is not running, a warning is logged and no action is taken.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        logger.LogMethod(LogLevel.Information, "Signalling graceful shutdown sequence to the outbox processing loop for {MessageType}.", args: [_messageTypeName]);

        await _cts.CancelAsync();

        if (_loopTask is not null)
        {
            try
            {
                await _loopTask.WaitAsync(cancellationToken);
                logger.LogMethod(LogLevel.Information, "Outbox processing loop for {MessageType} successfully brought to a standstill.", args: [_messageTypeName]);
            }
            catch (OperationCanceledException)
            {
                logger.LogMethod(LogLevel.Warning, "Host cancellation timeframe expired before the outbox task for {MessageType} could run to completion. Pipeline execution was severed.", args: [_messageTypeName]);
            }
        }
    }

    /// <summary>
    /// Releases all resources used by the outbox pipeline, including cancellation token sources and any background tasks. This method should be called when the pipeline is no longer needed to ensure proper cleanup.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async ValueTask DisposeAsync()
    {
        logger.LogMethod(LogLevel.Debug, "Releasing internal resources and token sources for outbox pipeline: {MessageType}.", args: [_messageTypeName]);

        await _cts.CancelAsync();

        if (_loopTask is not null)
        {
            try { await _loopTask; }
            catch (OperationCanceledException) { }
        }

        _linkedCts?.Dispose();
        _cts.Dispose();

        GC.SuppressFinalize(this);
    }
}