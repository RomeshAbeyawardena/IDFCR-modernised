namespace IDFCR.Abstractions.Caching;

/// <summary>
/// Represents a collection of cache groups that can be managed together. This interface extends IReadOnlyDictionary, allowing for the retrieval of cache groups by their unique keys. It also provides methods to assign and remove cache keys from specific groups, enabling organized management of related cache entries. The interface ensures that cache groups can be accessed and modified in a structured manner, facilitating efficient caching strategies.
/// </summary>
public interface ICacheGroups : IReadOnlyDictionary<string, ICacheGroup>
{
    /// <summary>
    /// Retrieves the cache keys associated with the cache group identified by the given key. This method returns an enumerable collection of cache keys that are part of the specified cache group. If the group does not exist, an empty collection is returned. This allows for efficient enumeration of cache keys within a specific group.
    /// </summary>
    /// <param name="key">The unique key that identifies the cache group whose cache keys should be retrieved.</param>
    /// <returns>An enumerable collection of cache keys associated with the specified cache group.</returns>
    IEnumerable<string> GetCacheKeys(string key);
    /// <summary>
    /// Determines whether the specified cache key is associated with the cache group identified by the given key. This method checks if the cache group exists and if the provided cache key is part of that group's list of associated cache keys. It returns true if the cache key is found within the group; otherwise, it returns false. This allows for efficient verification of cache key membership within specific groups.
    /// </summary>
    /// <param name="key">The unique key that identifies the cache group to check.</param>
    /// <param name="cacheKey">The cache key to verify within the specified cache group.</param>
    /// <returns>True if the cache key is found within the group; otherwise, false.</returns>
    bool HasCacheKey(string key, string cacheKey);
    /// <summary>
    /// Attempts to assign the specified cache keys to the cache group identified by the given key. If the group exists, the method adds the provided cache keys to the group's list of associated cache keys, ensuring that each key is unique within the group. The method returns true if at least one cache key was successfully added to the group; otherwise, it returns false. This allows for organized management of related cache entries by grouping them under a common identifier.
    /// </summary>
    /// <param name="key">The unique key that identifies the cache group to which the cache keys should be assigned.</param>
    /// <param name="cacheKeys">The cache keys to be assigned to the specified cache group.</param>
    /// <returns>True if at least one cache key was successfully added to the group; otherwise, false.</returns>
    bool TryAssignToGroup(string key, params string[] cacheKeys);
    /// <summary>
    /// Attempts to remove the specified cache keys from the cache group identified by the given key. If the group exists, the method removes the provided cache keys from the group's list of associated cache keys. The method returns true if at least one cache key was successfully removed from the group; otherwise, it returns false. This allows for organized management of related cache entries by removing specific keys from a common identifier.
    /// </summary>
    /// <param name="key">The unique key that identifies the cache group from which the cache keys should be removed.</param>
    /// <param name="cacheKeys">The cache keys to be removed from the specified cache group.</param>
    /// <returns>True if at least one cache key was successfully removed from the group; otherwise, false.</returns>
    bool TryRemoveFromGroup(string key, params string[] cacheKeys);
}
