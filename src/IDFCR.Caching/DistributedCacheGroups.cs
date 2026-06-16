using IDFCR.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace IDFCR.Caching;

/// <inheritdoc />
public class DistributedCacheGroups(IDistributedCache distributedCache, MessagePack.MessagePackSerializerOptions options) : IDistributedCacheGroups
{
    /// <inheritdoc />
    public ICacheGroups Groups { get; private set; } = new DefaultCacheGroups();

    /// <inheritdoc />
    public Task<byte[]?> GetAsync(string groupKey, string compositeKey, Func<string, string, string>? format, CancellationToken cancellationToken)
    {
        if (Groups.TryGetValue(groupKey, out var cacheGroup)
            && cacheGroup.CacheKeys.Contains(compositeKey))
        {
            return distributedCache.GetAsync(format?.Invoke(groupKey, compositeKey) ?? compositeKey, cancellationToken);
        }

        return Task.FromResult<byte[]?>(null);
    }

    /// <inheritdoc />
    public Task<byte[]?> GetAsync(string groupKey, string compositeKey, CancellationToken cancellationToken)
    {
        return GetAsync(groupKey, compositeKey, cancellationToken);
    }

    /// <inheritdoc />
    public async Task LoadAsync(CancellationToken cancellationToken)
    {
        var data = await distributedCache.GetAsync(nameof(DefaultCacheGroups), cancellationToken);

        if(data is null || data.Length < 1)
        {
            return;
        }

        using var memoryStream = new MemoryStream(data);
        var result = await MessagePack.MessagePackSerializer.DeserializeAsync<DefaultCacheGroups>(memoryStream, options, cancellationToken);
        
        if (result is not null)
        {
            Groups = result;
        }
    }

    /// <inheritdoc />
    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        await MessagePack.MessagePackSerializer.SerializeAsync(memoryStream, Groups, options, cancellationToken: cancellationToken);

        await distributedCache.SetAsync(nameof(DefaultCacheGroups), memoryStream.ToArray(), cancellationToken);
    }
}
