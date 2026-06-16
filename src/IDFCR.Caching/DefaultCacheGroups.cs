using IDFCR.Abstractions.Caching;
using System.Collections.Concurrent;

namespace IDFCR.Caching;

[MessagePack.MessagePackObject(true)]
internal class DefaultCacheGroups : ConcurrentDictionary<string, ICacheGroup>, ICacheGroups
{
    public bool TryAssignToGroup(string key, params string[] cacheKeys)
    {
        if (TryGetValue(key, out var cacheGroup))
        {
            int addedCount = 0;

            foreach(var itemKey in cacheKeys)
            {
                if (!cacheGroup.CacheKeys.Contains(itemKey))
                {
                    cacheGroup.CacheKeys.Add(itemKey);
                    addedCount++;
                }
            }

            return addedCount > 0;
        }

        return false;
    }

    public bool TryRemoveFromGroup(string key, params string[] cacheKeys)
    {
        if (TryGetValue(key, out var cacheGroup))
        {
            int removedCount = 0;

            foreach (var itemKey in cacheKeys)
            {
                if (cacheGroup.CacheKeys.Remove(itemKey))
                {
                    removedCount++;
                }
            }

            return removedCount > 0;
        }

        return false;
    }
}
