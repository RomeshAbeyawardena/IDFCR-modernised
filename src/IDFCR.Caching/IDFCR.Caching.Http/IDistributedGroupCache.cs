namespace IDFCR.Caching.Http;

/// <summary>
/// Represents a distributed cache service that supports grouping of cache entries. This interface defines methods for retrieving and storing cached data associated with specific group keys and composite keys, allowing for organized management of related cache items. It provides functionality to get and set cached values, as well as retrieve the collection of cache keys within a specified group.
/// </summary>
public interface IDistributedGroupCache
{
    /// <summary>
    /// Gets the cached value associated with the specified group key and composite key, using a custom format function to generate the cache key. This method retrieves the cached data for a specific cache entry within a designated cache group, allowing for efficient access to related cache items. If the specified group or composite key does not exist, the method returns null.
    /// </summary>
    /// <param name="groupKey">The key identifying the cache group.</param>
    /// <param name="compositeKey">The key identifying the specific cache entry within the group.</param>
    /// <param name="format">A function to format the cache key.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The cached value as a byte array, or null if not found.</returns>
    Task<byte[]?> GetAsync(string groupKey, string compositeKey, Func<string, string, string>? format, CancellationToken cancellationToken);
    /// <summary>
    /// Gets the cached value associated with the specified group key and composite key. This method retrieves the cached data for a specific cache entry within a designated cache group, allowing for efficient access to related cache items. If the specified group or composite key does not exist, the method returns null.
    /// </summary>
    /// <param name="groupKey">The key identifying the cache group.</param>
    /// <param name="compositeKey">The key identifying the specific cache entry within the group.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The cached value as a byte array, or null if not found.</returns>
    Task<byte[]?> GetAsync(string groupKey, string compositeKey, CancellationToken cancellationToken);
    /// <summary>
    /// Sets the cached value associated with the specified group key and composite key, using a custom format function to generate the cache key. This method stores the provided data for a specific cache entry within a designated cache group, allowing for efficient management of related cache items. If the specified group or composite key does not exist, it will be created.
    /// </summary>
    /// <param name="groupKey">The key identifying the cache group.</param>
    /// <param name="compositeKey">The key identifying the specific cache entry within the group.</param>
    /// <param name="format">A function to format the cache key.</param>
    /// <param name="data">The data to be cached.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetAsync(string groupKey, string compositeKey, Func<string, string, string>? format, byte[] data, CancellationToken cancellationToken);
    /// <summary>
    /// Sets the cached value associated with the specified group key and composite key. This method stores the provided data for a specific cache entry within a designated cache group, allowing for efficient management of related cache items. If the specified group or composite key does not exist, it will be created.
    /// </summary>
    /// <param name="groupKey">The key identifying the cache group.</param>
    /// <param name="compositeKey">The key identifying the specific cache entry within the group.</param>
    /// <param name="data">The data to be cached.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetAsync(string groupKey, string compositeKey, byte[] data, CancellationToken cancellationToken);
    /// <summary>
    /// Retrieves the keys of all cache entries within the specified cache group.
    /// </summary>
    /// <param name="groupKey">The key identifying the cache group.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing the keys of all cache entries within the group.</returns>
    Task<IEnumerable<string>> GetCacheKeysAsync(string groupKey, CancellationToken cancellationToken);
}
