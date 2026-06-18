using Microsoft.Extensions.Logging;

namespace IDFCR.Caching.Http.Auditing;

/// <summary>
/// Represents a sink for auditing distributed cache events. This interface defines a method for recording cache events, allowing for tracking and logging of cache operations. Implementations of this interface can be used to capture and store audit information related to distributed cache activities, facilitating monitoring and analysis of cache usage patterns.
/// </summary>
public interface IDistributedGroupCacheAuditSink
{
    /// <summary>
    /// Gets the default log level to be used for auditing distributed cache events. This property indicates the severity or importance of the events being recorded, allowing for appropriate logging and filtering of cache-related events.
    /// </summary>
    LogLevel DefaultLogLevel { get; }
    /// <summary>
    /// Records a distributed cache event for auditing purposes. This method allows for capturing and logging information about cache operations, enabling tracking of cache usage and facilitating analysis of cache behavior. Implementations of this method can store audit information in various formats, such as logs, databases, or other storage mechanisms.
    /// </summary>
    /// <param name="cacheEvent">The cache event to be recorded.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous record operation.</returns>
    Task RecordAsync(IDistributedCacheEvent cacheEvent, CancellationToken cancellationToken);
}
