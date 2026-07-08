
using IDFCR.Abstractions.Outbox;
using IDFCR.Abstractions.Results;

namespace IDFCR.Outbox.Extensions.Dispatchers;

/// <inheritdoc cref="IOutboxPipeline"/>
public abstract class OutboxPipelineBase<TMessage, TPagedQuery>(
    IOutboxReaderFactory<TMessage> outboxReaderFactory,
    IOutboxDispatcher<TMessage, TPagedQuery> outboxDispatcher,
    int delay = 1000) : IOutboxPipeline
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
        var readers = outboxReaderFactory.GetCompatibleReaders<TPagedQuery>();

        foreach(var reader in readers)
        {
            await reader.GetMessagesAsync(SetFilters(0, 20), cancellationToken);
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