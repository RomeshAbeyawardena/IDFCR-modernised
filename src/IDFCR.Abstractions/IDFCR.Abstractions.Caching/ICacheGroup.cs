namespace IDFCR.Abstractions.Caching;

/// <summary>
/// Represents a group of cache keys that can be managed together. Each cache group has a unique key and a list of associated cache keys. This interface defines the properties that a cache group must implement, allowing for the organization and management of related cache entries in a structured manner.
/// </summary>
public interface ICacheGroup
{
    /// <summary>
    /// Gets the unique key that identifies this cache group. This key is used to reference the group and manage its associated cache keys. Each cache group must have a distinct key to ensure proper organization and retrieval of related cache entries.
    /// </summary>
    string Key { get; }
    /// <summary>
    /// Gets the list of cache keys associated with this cache group. These keys represent the individual cache entries that belong to the group, allowing for collective management and operations on related cache items. The list can be used to add, remove, or retrieve cache keys as needed.
    /// </summary>
    IList<string> CacheKeys { get; }
}
