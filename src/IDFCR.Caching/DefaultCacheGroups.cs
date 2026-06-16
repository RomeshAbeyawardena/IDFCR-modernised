using IDFCR.Abstractions.Caching;
using System.Collections.Concurrent;

namespace IDFCR.Caching;

[MessagePack.MessagePackObject(true)]
internal class DefaultCacheGroups : ConcurrentDictionary<string, ICacheGroup>, ICacheGroups
{
    public bool TryAssignToGroup(string key, params string[] cacheKeys)
    {
        // Get or add the group atomically
        var cacheGroup = GetOrAdd(key, _ => new DefaultCacheGroup { Key = key });

        bool mutated = false;

        // Lock on the individual group to prevent concurrent collection corruption
        lock (cacheGroup)
        {
            foreach (var itemKey in cacheKeys)
            {
                if (!cacheGroup.CacheKeys.Contains(itemKey))
                {
                    cacheGroup.CacheKeys.Add(itemKey);
                    mutated = true;
                }
            }
        }

        return mutated;
    }

    public bool TryRemoveFromGroup(string key, params string[] cacheKeys)
    {
        if (TryGetValue(key, out var cacheGroup))
        {
            int removedCount = 0;

            lock (cacheGroup)
            {
                foreach (var itemKey in cacheKeys)
                {
                    if (cacheGroup.CacheKeys.Remove(itemKey))
                    {
                        removedCount++;
                    }
                }
            }

            return removedCount > 0;
        }

        return false;
    }
}