using IDFCR.Abstractions.Caching;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace IDFCR.Caching;

/// <inheritdoc cref="ICacheGroups" />
[Obsolete("Not to be used in consumer code")]
[EditorBrowsable(EditorBrowsableState.Never)]
[MessagePack.MessagePackObject(true)]
public sealed partial class DefaultCacheGroups : ICacheGroups
{
    private readonly ConcurrentDictionary<string, ICacheGroupWithLock> cacheGroups = new();

    /// <summary>
    /// Gets or sets an externally exposed dictionary of cache groups. This property allows for MessagePack serialization and deserialization of the cache groups, enabling the persistence and retrieval of cache group data. The dictionary is read-only, ensuring that external code cannot modify the internal state of the cache groups directly. When setting this property, the internal collection is cleared and repopulated with the provided values, maintaining thread safety and consistency.
    /// </summary>
    [MessagePack.Key(0)]
    public IReadOnlyDictionary<string, DefaultCacheGroup> CacheGroups
    {
        get => cacheGroups.ToDictionary(x => x.Key, x => (DefaultCacheGroup)x.Value);
        set
        {
            cacheGroups.Clear();
            foreach(var (key, val) in value)
            {
                cacheGroups.TryAdd(key, val);
            }
        }
    }

    ICacheGroup IReadOnlyDictionary<string, ICacheGroup>.this[string key] => cacheGroups[key];

    IEnumerable<string> IReadOnlyDictionary<string, ICacheGroup>.Keys => cacheGroups.Keys;
    IEnumerable<ICacheGroup> IReadOnlyDictionary<string, ICacheGroup>.Values => cacheGroups.Values;
    int IReadOnlyCollection<KeyValuePair<string, ICacheGroup>>.Count => cacheGroups.Count;

    /// <inheritdoc />
    public bool HasCacheKey(string key, string cacheKey)
    {
        if (cacheGroups.TryGetValue(key, out var cacheGroup))
        {
            lock(cacheGroup.Lock)
            {
                return cacheGroup.CacheKeys.Contains(cacheKey);
            }
        }

        return false;
    }
    /// <inheritdoc />
    public bool TryAssignToGroup(string key, params string[] cacheKeys)
    {
        // Get or add the group atomically
        var cacheGroup = cacheGroups.GetOrAdd(key, _ => new DefaultCacheGroup { Key = key });

        bool mutated = false;
        
        // Lock on the individual group to prevent concurrent collection corruption
        lock (cacheGroup.Lock)
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
    /// <inheritdoc />
    public bool TryRemoveFromGroup(string key, params string[] cacheKeys)
    {
        if (cacheGroups.TryGetValue(key, out var cacheGroup))
        {
            int removedCount = 0;

            lock (cacheGroup.Lock)
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

    bool IReadOnlyDictionary<string, ICacheGroup>.ContainsKey(string key)
    {
        return cacheGroups.ContainsKey(key);
    }
    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, ICacheGroup>> GetEnumerator()
    {
        foreach (var pair in cacheGroups)
        {
            yield return new KeyValuePair<string, ICacheGroup>(pair.Key, pair.Value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    bool IReadOnlyDictionary<string, ICacheGroup>.TryGetValue(string key, out ICacheGroup value)
    {
        if (cacheGroups.TryGetValue(key, out var val))
        {
            value = val;
            return true;
        }

        value = null!;
        return false;
    }
    /// <inheritdoc />
    public IEnumerable<string> GetCacheKeys(string key)
    {
        if (cacheGroups.TryGetValue(key, out var cacheGroup))
        {
            lock (cacheGroup.Lock)
            {
                return [.. cacheGroup.CacheKeys];
            }
        }

        return [];
    }
}