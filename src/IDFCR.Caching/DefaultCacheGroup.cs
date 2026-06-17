using IDFCR.Abstractions.Caching;

namespace IDFCR.Caching;

/// <inheritdoc cref="ICacheGroup"/>
[Obsolete("Not to be used in consumer code"),
 MessagePack.MessagePackObject(false, AllowPrivate = true)]
public record DefaultCacheGroup : ICacheGroupWithLock
{
    /// <inheritdoc/>
    [MessagePack.Key(0)]
    public required string Key { get; init; }
    /// <inheritdoc/>
    [MessagePack.Key(1)]
    public IList<string> CacheKeys { get; } = [];
    /// <inheritdoc/>
    [MessagePack.IgnoreMember]
    public Lock @Lock { get; } = new();
}
