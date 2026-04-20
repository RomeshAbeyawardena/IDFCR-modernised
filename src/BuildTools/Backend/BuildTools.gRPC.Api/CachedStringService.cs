namespace BuildTools.GRPC.Api;

public sealed class CachedStringService(int? initialCount = null, int? maxCount = null) : IDisposable
{
    private readonly SemaphoreSlim semaphoreSlim = new(initialCount.GetValueOrDefault(1), maxCount.GetValueOrDefault(1));
    private string? _value;
    public async Task<string?> GetCachedValueAsync(CancellationToken? cancellationToken = null)
    {
        try
        {
            await semaphoreSlim.WaitAsync(cancellationToken.GetValueOrDefault(CancellationToken.None));
            return _value;
        }
        finally
        {
            semaphoreSlim.Release(1);
        }
    }

    public async Task<string?> SetCachedValueAsync(string value, CancellationToken? cancellationToken = null)
    {
        try
        {
            await semaphoreSlim.WaitAsync(cancellationToken.GetValueOrDefault(CancellationToken.None));
            var oldValue = value;
            _value = value;

            return oldValue;
        }
        finally
        {
            semaphoreSlim.Release(1);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        semaphoreSlim?.Dispose();
    }
}
