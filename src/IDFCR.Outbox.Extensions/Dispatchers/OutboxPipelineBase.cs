
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
    where TMessage: IOutboxEntity
    where TPagedQuery : IPagedQuery, new()
{
    private readonly CancellationTokenSource _cts = new();
    private CancellationTokenSource? _linkedCts;
    private Task? _loopTask;

    /// <summary>
    /// Sets the filters for the outbox reader. This method can be overridden to provide custom filtering logic.
    /// <para>Use <code>with {}</code> to customise the query properties that normally are unavailable to set outside an init.</para>
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public virtual TPagedQuery SetFilters(int pageIndex, int pageSize)
    {
        var query = new TPagedQuery()
        {
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        return query;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // 1. Resolve readers locally
        var readers = outboxReaderFactory.GetCompatibleReaders<TPagedQuery>();
        logger.LogMethod(LogLevel.Information, "Executing outbox pipeline with {ReaderCount} readers", args: readers.Count());
        foreach (var reader in readers)
        {
            int currentPage = 0;
            bool hasMorePages = true;

            // 2. Loop through pages for the current reader
            while (hasMorePages && !cancellationToken.IsCancellationRequested)
            {
                var query = SetFilters(currentPage, pageSize);

                // 3. Fetch the messages
                var messages = await reader.GetMessagesAsync(query, cancellationToken);

                await outboxDispatcher.PushAsync(messages, cancellationToken);

                // 5. Determine if we need to fetch another page
                // If the number of messages returned is less than the page size, 
                // we have definitively reached the end of the data.
                if ((messages?.Result?.Count() ?? 0) < pageSize)
                {
                    hasMorePages = false;
                }
                else
                {
                    // If we got exactly the page size, there MIGHT be more data, so increment and repeat
                    currentPage++;
                }
            }
        }
    }

    /// <inheritdoc />
    private async Task LoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ExecuteAsync(cancellationToken);
                await Task.Delay(delay, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception)
        {
            // log critical
            throw;
        }
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_loopTask is { IsCompleted: false })
            return Task.CompletedTask;

        _linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            _cts.Token,
            cancellationToken);

        _loopTask = Task.Run(() => LoopAsync(_linkedCts.Token), CancellationToken.None);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        await _cts.CancelAsync();

        if (_loopTask is not null)
        {
            try
            {
                await _loopTask.WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Either the loop stopped normally, or the host stopped waiting.
            }
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
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