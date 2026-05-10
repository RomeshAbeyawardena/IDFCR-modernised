using FastMember;
using System.Collections.Concurrent;

namespace IDFCR.Results.Http.Extensions;

public static class ObjectExtensions
{
    private static readonly Lazy<ConcurrentDictionary<Type, string[]>> _mappableMemberCache = new([]);
    public static IReadOnlyDictionary<string, string?> ToDictionary<T>(this T source)
    {
        Dictionary<string, string?> itemValueDictionary = [];
        var type = source.GetType();
        var sourceAccessor = TypeAccessor.Create(type);
        var sourceTypeNames = _mappableMemberCache.Value.GetOrAdd(type, (t) => [.. sourceAccessor.GetMembers().Where(m => m.CanWrite && m.CanRead).Select(m => m.Name)]);

        foreach (var name in sourceTypeNames)
        {
            var s = sourceAccessor[source, name];
            itemValueDictionary.TryAdd(name, s?.ToString());
        }

        return itemValueDictionary;
    }
}
