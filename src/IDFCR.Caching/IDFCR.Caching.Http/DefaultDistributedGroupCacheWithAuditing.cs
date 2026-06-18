using IDFCR.Abstractions.Caching;
using IDFCR.Caching.Http.Auditing;

namespace IDFCR.Caching.Http;

internal class DefaultDistributedGroupCacheWithAuditing(
    IDistributedCacheGroups distributedCache, 
    IDistributedGroupCacheAuditSink cacheAuditSink,
    TimeProvider timeProvider) : DefaultDistributedGroupCache(distributedCache)
{
    public override async Task<byte[]?> GetAsync(string groupKey, string compositeKey, CancellationToken cancellationToken)
    {
        var result = await base.GetAsync(groupKey, compositeKey, cancellationToken);

        await cacheAuditSink.RecordAsync(new DistributedCacheEvent
        {
            Operation = AuditOperations.Get,
            GroupKey = groupKey,
            CompositeKey = compositeKey,
            Success = result is not null,
            Timestamp = timeProvider.GetUtcNow()
        }, cancellationToken);

        return result;
    }

    public override async Task<byte[]?> GetAsync(string groupKey, string compositeKey, Func<string, string, string>? format, CancellationToken cancellationToken)
    {
        var result = await base.GetAsync(groupKey, compositeKey, format, cancellationToken);

        await cacheAuditSink.RecordAsync(new DistributedCacheEvent
        {
            Operation = AuditOperations.Get,
            GroupKey = groupKey,
            CompositeKey = compositeKey,
            Success = result is not null,
            Timestamp = timeProvider.GetUtcNow()
        }, cancellationToken);

        return result;
    }

    public override async Task<bool> RemoveAsync(string group, CancellationToken cancellationToken)
    {
        var cacheKeys = await GetCacheKeysAsync(group, cancellationToken);

        var result = await base.RemoveAsync(group, cancellationToken);

        await cacheAuditSink.RecordAsync(new DistributedCacheEvent
        {
            Operation = AuditOperations.Remove,
            GroupKey = group,
            CompositeKey = string.Join(',', cacheKeys),
            Success = result,
            Timestamp = timeProvider.GetUtcNow()
        }, cancellationToken);

        return result;
    }

    public override async Task SetAsync(string groupKey, string compositeKey, byte[] data, CancellationToken cancellationToken)
    {
        await base.SetAsync(groupKey, compositeKey, data, cancellationToken);

        await cacheAuditSink.RecordAsync(new DistributedCacheEvent
        {
            Operation = AuditOperations.Set,
            GroupKey = groupKey,
            CompositeKey = compositeKey,
            Success = true,
            Timestamp = timeProvider.GetUtcNow()
        }, cancellationToken);
    }

    public override async Task SetAsync(string groupKey, string compositeKey, Func<string, string, string>? format, byte[] data, CancellationToken cancellationToken)
    {
        await base.SetAsync(groupKey, compositeKey, format, data, cancellationToken);

        await cacheAuditSink.RecordAsync(new DistributedCacheEvent
        {
            Operation = AuditOperations.Set,
            GroupKey = groupKey,
            CompositeKey = compositeKey,
            Success = true,
            Timestamp = timeProvider.GetUtcNow()
        }, cancellationToken);
    }
}
