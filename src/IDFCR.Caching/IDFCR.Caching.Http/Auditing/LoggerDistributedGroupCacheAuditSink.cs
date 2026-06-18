using Microsoft.Extensions.Logging;

namespace IDFCR.Caching.Http.Auditing;

internal class LoggerDistributedGroupCacheAuditSink(ILogger<IDistributedGroupCacheAuditSink> logger) : IDistributedGroupCacheAuditSink
{
    public LogLevel DefaultLogLevel { get; } = LogLevel.Information;

    public Task RecordAsync(IDistributedCacheEvent cacheEvent, CancellationToken cancellationToken)
    {
        var logLevel = cacheEvent.LogLevel ?? DefaultLogLevel;

        if (logger.IsEnabled(logLevel))
        {
            logger.Log(logLevel, "{Timestamp}: Distributed cache event: {Operation} for group {GroupKey} and composite key {CompositeKey}",
                cacheEvent.Timestamp, cacheEvent.Operation, cacheEvent.GroupKey, cacheEvent.CompositeKey);
        }

        return Task.CompletedTask;
    }
}
