using Microsoft.Extensions.Logging;

namespace IDFCR.Caching.Http.Auditing;

/// <summary>
/// Represents an event that occurs during a distributed cache operation. This interface defines properties that provide information about the cache operation, including the type of operation performed, the group key and composite key involved, the success status of the operation, and the timestamp when the event occurred. It serves as a contract for capturing and recording cache events for auditing or logging purposes.
/// </summary>
public interface IDistributedCacheEvent
{
    /// <summary>
    /// Gets the log level associated with the cache event. This property indicates the severity or importance of the event, allowing for appropriate logging and filtering of cache-related events.
    /// </summary>
    LogLevel? LogLevel { get; }
    /// <summary>
    /// Gets the type of cache operation that was performed (e.g., "Get", "Set", "Remove"). This property indicates the specific action taken on the distributed cache, allowing for identification and categorization of cache events.
    /// </summary>
    string Operation { get; }

    /// <summary>
    /// Gets the key that identifies the cache group involved in the operation.
    /// </summary>
    string GroupKey { get; }

    /// <summary>
    /// Gets the composite key that identifies the specific cache entry within the group, if applicable.
    /// </summary>
    string? CompositeKey { get; }

    /// <summary>
    /// Gets the outcome of the cache operation, indicating whether it was successful or not. This property provides information about the result of the cache operation, allowing for tracking and analysis of cache performance and reliability.
    /// </summary>
    string Outcome { get; }

    /// <summary>
    /// Gets the timestamp when the cache event occurred.
    /// </summary>
    DateTimeOffset Timestamp { get; }
}
