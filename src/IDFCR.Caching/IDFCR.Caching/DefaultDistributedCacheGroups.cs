using IDFCR.Abstractions.Caching;
using IDFCR.Caching.Serialisation.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace IDFCR.Caching;

/// <inheritdoc />
internal class DefaultDistributedCacheGroups(IDistributedCache distributedCache, MessagePack.MessagePackSerializerOptions options) : IDistributedCacheGroups
{
#pragma warning disable CS0618

    internal async Task<DefaultCacheGroups> DeserializeAsync(byte[] data, CancellationToken cancellationToken)
    {
        
        return await data
            .DeserialiseAsync<DefaultCacheGroups>(options, cancellationToken);
    }

    internal async Task<byte[]> SerializeAsync(ICacheGroups groups, CancellationToken cancellationToken)
    {
        var allocGroups = (DefaultCacheGroups)groups;
        return await allocGroups.SerialiseAsync(options, cancellationToken);
    }

    /// <inheritdoc />
    public ICacheGroups Groups { get; private set; } = new DefaultCacheGroups();

    /// <inheritdoc />
    public Task<byte[]?> GetAsync(string groupKey, string compositeKey, Func<string, string, string>? format, CancellationToken cancellationToken)
    {
        if (Groups.HasCacheKey(groupKey, compositeKey))
        {
            return distributedCache.GetAsync(format?.Invoke(groupKey, compositeKey) ?? compositeKey, cancellationToken);
        }

        return Task.FromResult<byte[]?>(null);
    }

    /// <inheritdoc />
    public Task<byte[]?> GetAsync(string groupKey, string compositeKey, CancellationToken cancellationToken)
    {
        return GetAsync(groupKey, compositeKey, null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task LoadAsync(CancellationToken cancellationToken)
    {

        var data = await distributedCache.GetAsync(nameof(DefaultCacheGroups), cancellationToken);

        if(data is null || data.Length < 1)
        {
            return;
        }

        
        var result = await DeserializeAsync(data, cancellationToken);
        
        if (result is not null)
        {
            Groups = result;
        }
    }

    /// <inheritdoc />
    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        var data = await SerializeAsync(Groups, cancellationToken);
        await distributedCache.SetAsync(nameof(DefaultCacheGroups), data, cancellationToken);
    }

    public async Task SetAsync(string groupKey, string compositeKey, Func<string, string, string>? format, byte[] data, CancellationToken cancellationToken)
    {
        try
        {
            var cachedKey = format?.Invoke(groupKey, compositeKey) ?? compositeKey;

            if (Groups.TryAssignToGroup(groupKey, compositeKey))
            {
                await distributedCache.SetAsync(cachedKey, data, cancellationToken);
            }
        }
        catch
        {
            Groups.TryRemoveFromGroup(groupKey, compositeKey);
        }
    }

    public Task SetAsync(string groupKey, string compositeKey, byte[] data, CancellationToken cancellationToken)
    {
        return SetAsync(groupKey, compositeKey, null, data, cancellationToken);
    }

    public async Task<IEnumerable<string>> GetCacheKeysAsync(string groupKey, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        if (Groups.TryGetValue(groupKey, out var cacheGroup))
        {
            return cacheGroup.CacheKeys;
        }

        return [];
    }

    public async Task<bool> RemoveAsync(string group, CancellationToken cancellationToken)
    {
        var cacheKeys = await GetCacheKeysAsync(group, cancellationToken);

        if (cacheKeys is null)
        {
            return false;
        }

        foreach (var key in cacheKeys)
        {
            distributedCache.Remove(key);
        }

        return Groups.TryRemoveFromGroup(group, [.. cacheKeys]);
    }
}
#pragma warning restore