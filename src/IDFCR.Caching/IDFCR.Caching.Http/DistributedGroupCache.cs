using IDFCR.Abstractions.Caching;

namespace IDFCR.Caching.Http;

internal class DefaultDistributedGroupCache(IDistributedCacheGroups distributedCache) : IDistributedGroupCache
{
    public async Task<byte[]?> GetAsync(string groupKey, string compositeKey, Func<string, string, string>? format, CancellationToken cancellationToken)
    {
        await distributedCache.LoadAsync(cancellationToken);
        return await distributedCache.GetAsync(groupKey, compositeKey, format, cancellationToken);
    }

    public async Task<byte[]?> GetAsync(string groupKey, string compositeKey, CancellationToken cancellationToken)
    {
        await distributedCache.LoadAsync(cancellationToken);
        return await distributedCache.GetAsync(groupKey, compositeKey, cancellationToken);
    }

    public async Task<IEnumerable<string>> GetCacheKeysAsync(string groupKey, CancellationToken cancellationToken)
    {
        await distributedCache.LoadAsync(cancellationToken);
        return await distributedCache.GetCacheKeysAsync(groupKey, cancellationToken);
    }

    public Task<bool> RemoveAsync(string group, CancellationToken cancellationToken)
    {
        return distributedCache.RemoveAsync(group, cancellationToken);
    }

    public async Task SetAsync(string groupKey, string compositeKey, Func<string, string, string>? format, byte[] data, CancellationToken cancellationToken)
    {
        await distributedCache.LoadAsync(cancellationToken);
        await distributedCache.SetAsync(groupKey, compositeKey, format, data, cancellationToken);
        await distributedCache.SaveAsync(cancellationToken);
    }

    public async Task SetAsync(string groupKey, string compositeKey, byte[] data, CancellationToken cancellationToken)
    {
        await distributedCache.LoadAsync(cancellationToken);
        await distributedCache.SetAsync(groupKey, compositeKey, data, cancellationToken);
        await distributedCache.SaveAsync(cancellationToken);
    }
}
