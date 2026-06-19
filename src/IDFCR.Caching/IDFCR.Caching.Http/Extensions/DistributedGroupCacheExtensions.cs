using IDFCR.Caching.Serialisation.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace IDFCR.Caching.Http.Extensions;

/// <summary>
/// Define extension methods for the IDistributedGroupCache interface, providing a convenient way to retrieve or set cached items with serialization and deserialization support. These methods enhance the functionality of the distributed group cache by allowing developers to easily manage cached data in a type-safe manner, leveraging MessagePack for efficient serialization.
/// </summary>
public static class DistributedGroupCacheExtensions
{
    /// <summary>
    /// Gets an item from the distributed group cache or sets it if it does not exist. This method attempts to retrieve the cached item using the specified group and composite keys. If the item is not found, it serializes the provided item using MessagePack and stores it in the cache for future retrievals.
    /// </summary>
    /// <typeparam name="T">The type of the item to be retrieved or set in the cache.</typeparam>
    /// <param name="cache">The distributed group cache instance.</param>
    /// <param name="groupKey">The key representing the group in the cache.</param>
    /// <param name="compositeKey">The composite key used to identify the specific item within the group.</param>
    /// <param name="options">The MessagePack serializer options.</param>
    /// <param name="item">The item to be cached if it does not exist in the cache.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The item retrieved from the cache or the provided item if it was not found in the cache.</returns>
    public static async Task<T> GetOrSetAsync<T>(
        this IDistributedGroupCache cache,
        string groupKey,
        string compositeKey,
        MessagePack.MessagePackSerializerOptions options,
        T item,
        CancellationToken cancellationToken)
    {
        var packedResult = await cache.GetAsync(groupKey, compositeKey, cancellationToken);

        if (packedResult is null || packedResult.Length < 1)
        {
            if (item is null)
            {
                return item; //we know this is null, so we can return it without any drama.
            }

            packedResult = await item.SerialiseAsync(options, cancellationToken);
            await cache.SetAsync(groupKey, compositeKey, packedResult, cancellationToken);

            return item;
        }

        return await packedResult.DeserialiseAsync<T>(options, cancellationToken);
    }

    /// <summary>
    /// Gets an item from the distributed group cache or sets it if it does not exist. This method attempts to retrieve the cached item using the specified group and composite keys. If the item is not found, it invokes the provided factory function to obtain the item, serializes it using MessagePack, and stores it in the cache for future retrievals.
    /// <para>Be wary when the <paramref name="getItemFactory"/> callback invokes other methods that require a scoped context, as it may not be executed immediately if the injected <see cref="IDistributedCache"/> is dependent on an external resource that may take time to complete.</para>
    /// </summary>
    /// <typeparam name="T">The type of the item to be retrieved or set in the cache.</typeparam>
    /// <param name="cache">The distributed group cache instance.</param>
    /// <param name="groupKey">The key representing the group in the cache.</param>
    /// <param name="compositeKey">The composite key used to identify the specific item within the group.</param>
    /// <param name="options">The MessagePack serializer options.</param>
    /// <param name="getItemFactory">A factory function to obtain the item if it is not found in the cache.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The item retrieved from the cache or obtained from the factory function.</returns>
    public static async Task<T> GetOrSetAsync<T>(
        this IDistributedGroupCache cache, 
        string groupKey, 
        string compositeKey, 
        MessagePack.MessagePackSerializerOptions options, 
        Func<CancellationToken, Task<T>> getItemFactory, 
        CancellationToken cancellationToken)
    {
        var packedResult = await cache.GetAsync(groupKey, compositeKey, cancellationToken);

        if (packedResult is null || packedResult.Length < 1)
        {
            var item = await getItemFactory(cancellationToken);
            if (item is null)
            {
                return item;
            }

            packedResult = await item.SerialiseAsync(options, cancellationToken);
            await cache.SetAsync(groupKey, compositeKey, packedResult, cancellationToken);

            return item;
        }

        return await packedResult.DeserialiseAsync<T>(options, cancellationToken);
    }
}
