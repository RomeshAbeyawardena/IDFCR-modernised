using IDFCR.Abstractions.Caching;

namespace IDFCR.Caching;

[MessagePack.MessagePackObject(false, AllowPrivate = true)]
internal class DefaultCacheGroup : ICacheGroupWithLock
{
    [MessagePack.Key(0)]
    public required string Key { get; set; }
    [MessagePack.Key(1)]
    public IList<string> CacheKeys { get; set; } = [];
    [MessagePack.IgnoreMember]
    public Lock @Lock { get; } = new();
}
