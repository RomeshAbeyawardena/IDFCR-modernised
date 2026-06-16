using IDFCR.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace IDFCR.Caching;

/// <inheritdoc />
public class DistributedCacheGroups(IDistributedCache distributedCache, MessagePack.MessagePackSerializerOptions options) : IDistributedCacheGroups
{
    /// <inheritdoc />
    public ICacheGroups Groups { get; private set; } = new DefaultCacheGroups();

    /// <inheritdoc />
    public async Task LoadAsync(CancellationToken cancellationToken)
    {
        var data = await distributedCache.GetAsync(nameof(DefaultCacheGroups), cancellationToken);

        if(data is null || data.Length < 1)
        {
            return;
        }

        using var memoryStream = new MemoryStream();
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
