namespace IDFCR.Abstractions.Caching;

/// <summary>
/// Represents a collection of cache groups that can be managed together. This interface defines methods for assigning and removing cache keys from specific groups, allowing for organized management of related cache entries. It provides functionality to add or remove cache keys from groups, enabling efficient handling of cache data in a structured manner.
/// </summary>
public interface IDistributedCacheGroups
{
    /// <summary>
    /// Gets the cached value associated with the specified group key and composite key, using a custom format function to generate the cache key. This method retrieves the cached data for a specific cache entry within a designated cache group, allowing for efficient access to related cache items. If the specified group or composite key does not exist, the method returns null.
    /// </summary>
    /// <param name="groupKey">The key that identifies the cache group.</param>
    /// <param name="compositeKey">The composite key that identifies the specific cache entry within the group.</param>
    /// <param name="format">A function to generate the cache key based on the group key and composite key.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous get operation. The task result contains the cached value as a byte array, or null if the specified group or composite key does not exist.</returns>
    public Task<byte[]?> GetAsync(string groupKey, string compositeKey, Func<string, string, string>? format, CancellationToken cancellationToken);

    /// <summary>
    /// Sets the cached value associated with the specified group key and composite key, using a custom format function to generate the cache key. This method stores the provided data for a specific cache entry within a designated cache group, allowing for efficient management of related cache items. If the specified group or composite key does not exist, it will be created.
    /// </summary>
    /// <param name="groupKey">The key that identifies the cache group.</param>
    /// <param name="compositeKey">The composite key that identifies the specific cache entry within the group.</param>
    /// <param name="format">A function to generate the cache key based on the group key and composite key.</param>
    /// <param name="data">The data to be cached.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous set operation.</returns>
    public Task SetAsync(string groupKey, string compositeKey, Func<string, string, string>? format, byte[] data, CancellationToken cancellationToken);

    /// <summary>
    /// Sets the cached value associated with the specified group key and composite key. This method stores the provided data for a specific cache entry within a designated cache group, allowing for efficient management of related cache items. If the specified group or composite key does not exist, it will be created.
    /// </summary>
    /// <param name="groupKey">The key that identifies the cache group.</param>
    /// <param name="compositeKey">The composite key that identifies the specific cache entry within the group.</param>
    /// <param name="data">The data to be cached.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous set operation.</returns>
    public Task SetAsync(string groupKey, string compositeKey, byte[] data, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the cached value associated with the specified group key and composite key. This method retrieves the cached data for a specific cache entry within a designated cache group, allowing for efficient access to related cache items. If the specified group or composite key does not exist, the method returns null.
    /// </summary>
    /// <param name="groupKey">The key that identifies the cache group.</param>
    /// <param name="compositeKey">The composite key that identifies the specific cache entry within the group.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous get operation. The task result contains the cached value as a byte array, or null if the specified group or composite key does not exist.</returns>
    Task<byte[]?> GetAsync(string groupKey, string compositeKey, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the collection of cache groups managed by this service. Each cache group is identified by a unique key and contains a list of associated cache keys. This property allows for access to the entire set of cache groups, enabling operations such as adding, removing, or retrieving cache keys within specific groups.
    /// </summary>
    ICacheGroups Groups { get; }
    /// <summary>
    /// Asynchronously loads the cache groups from the underlying distributed cache. This method retrieves the stored cache group data and populates the Groups property, allowing for subsequent operations on the cache groups. The operation can be canceled using the provided cancellation token.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous load operation.</returns>
    Task LoadAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Asynchronously saves the cache groups to the underlying distributed cache. This method persists the current state of the Groups property, allowing for subsequent retrieval and management of cache groups. The operation can be canceled using the provided cancellation token.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveAsync(CancellationToken cancellationToken);
}