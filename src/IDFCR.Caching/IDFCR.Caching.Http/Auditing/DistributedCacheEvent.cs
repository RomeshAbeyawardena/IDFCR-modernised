using Microsoft.Extensions.Logging;

namespace IDFCR.Caching.Http.Auditing;

internal record DistributedCacheEvent : IDistributedCacheEvent
{
    public required string Operation { get; init; }
    public required string GroupKey { get; init; }
    public string? CompositeKey { get; init; }
    public bool Success { get; init; }
    public DateTimeOffset Timestamp { get; init; }
    public LogLevel? LogLevel { get; init; }
}
