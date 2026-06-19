using IDFCR.Abstractions.Caching;

namespace IDFCR.Caching.Http;

internal class DefaultDistributedGroupCache(IDistributedCacheGroups distributedCache) : IDistributedGroupCache
{
    public virtual async Task<byte[]?> GetAsync(string groupKey, string compositeKey, Func<string, string, string>? format, CancellationToken cancellationToken)
    {
        await distributedCache.LoadAsync(cancellationToken);
        return await distributedCache.GetAsync(groupKey, compositeKey, format, cancellationToken);
    }

    public virtual async Task<byte[]?> GetAsync(string groupKey, string compositeKey, CancellationToken cancellationToken)
    {
        await distributedCache.LoadAsync(cancellationToken);
        return await distributedCache.GetAsync(groupKey, compositeKey, cancellationToken);
    }

    public virtual async Task<IEnumerable<string>> GetCacheKeysAsync(string groupKey, CancellationToken cancellationToken)
    {
        await distributedCache.LoadAsync(cancellationToken);
        return await distributedCache.GetCacheKeysAsync(groupKey, cancellationToken);
    }

    public virtual async Task<bool> RemoveAsync(string group, CancellationToken cancellationToken)
    {
        await distributedCache.LoadAsync(cancellationToken);
        var result = await distributedCache.RemoveAsync(group, cancellationToken);
        if (result)
        {
            await distributedCache.SaveAsync(cancellationToken);
        }
        return result;
    }

    public virtual async Task SetAsync(string groupKey, string compositeKey, Func<string, string, string>? format, byte[] data, CancellationToken cancellationToken)
    {
        await distributedCache.LoadAsync(cancellationToken);
        await distributedCache.SetAsync(groupKey, compositeKey, format, data, cancellationToken);
        await distributedCache.SaveAsync(cancellationToken);
    }

    public virtual async Task SetAsync(string groupKey, string compositeKey, byte[] data, CancellationToken cancellationToken)
    {
        await distributedCache.LoadAsync(cancellationToken);
        await distributedCache.SetAsync(groupKey, compositeKey, data, cancellationToken);
        await distributedCache.SaveAsync(cancellationToken);
    }
}
