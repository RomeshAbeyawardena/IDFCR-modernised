using Microsoft.Extensions.Logging;

namespace IDFCR.Caching.Http.Auditing;

internal class LoggerDistributedGroupCacheAuditSink(ILogger<IDistributedGroupCacheAuditSink> logger) : IDistributedGroupCacheAuditSink
{
    public async Task RecordAsync(IDistributedCacheEvent cacheEvent, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        if (logger.IsEnabled(cacheEvent.LogLevel))
        {
            logger.Log(cacheEvent.LogLevel, "{Timestamp}: Distributed cache event: {Operation} for group {GroupKey} and composite key {CompositeKey}",
                cacheEvent.Timestamp, cacheEvent.Operation, cacheEvent.GroupKey, cacheEvent.CompositeKey);
        }
    }
}
