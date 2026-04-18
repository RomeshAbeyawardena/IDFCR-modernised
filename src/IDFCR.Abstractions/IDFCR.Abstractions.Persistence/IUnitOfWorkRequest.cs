namespace IDFCR.Abstractions.Persistence;

/// <summary>
/// Represents a request that can be processed within a unit of work, indicating whether changes should be committed to the data store after processing. This interface is typically used in scenarios where multiple operations need to be performed as part of a single transaction, allowing the caller to specify whether the changes made during the processing of the request should be persisted or rolled back. By implementing this interface, requests can provide information about their intent regarding data persistence, enabling better control over transactional behavior in applications that use the unit of work pattern.
/// </summary>
public interface IUnitOfWorkRequest
{
    /// <summary>
    /// Gets a value indicating whether the changes made during the processing of this request should be committed to the data store. If this property returns true, it indicates that the changes should be saved after processing; if false, it indicates that the changes should not be persisted, allowing for scenarios where operations may be performed without affecting the underlying data store. This property is essential for managing transactional behavior in applications that utilize the unit of work pattern, providing a way to control whether changes are finalized or discarded based on the specific requirements of the request being processed.
    /// </summary>
    bool CommitChanges { get; }
}