namespace IDFCR.Abstractions.Caching;

/// <summary>
/// Represents a collection of cache groups that can be managed together. This interface defines methods for assigning and removing cache keys from specific groups, allowing for organized management of related cache entries. It provides functionality to add or remove cache keys from groups, enabling efficient handling of cache data in a structured manner.
/// </summary>
public interface IDistributedCacheGroups
{
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