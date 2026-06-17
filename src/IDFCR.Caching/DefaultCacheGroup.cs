using IDFCR.Abstractions.Caching;
using System.ComponentModel;

namespace IDFCR.Caching;

/// <inheritdoc cref="ICacheGroup"/>
[Obsolete("Not to be used in consumer code"),
 EditorBrowsable(EditorBrowsableState.Never),
 MessagePack.MessagePackObject(false, AllowPrivate = true)]
public partial record DefaultCacheGroup : ICacheGroupWithLock
{
    /// <inheritdoc/>
    [MessagePack.Key(0)]
    public required string Key { get; init; }
    /// <inheritdoc/>
    [MessagePack.Key(1)]
    public IList<string> CacheKeys { get; private set; } = [];
    /// <inheritdoc/>
    [MessagePack.IgnoreMember]
    public Lock @Lock { get; } = new();
}
