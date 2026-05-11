using FastMember;
using System.Collections.Concurrent;

namespace IDFCR.Results.Http.Extensions;

/// <summary>
/// Defines extension methods for converting objects into dictionaries. These extension methods allow you to easily convert an object's properties into a dictionary format, which can be useful for various purposes such as serialization, logging, or creating metadata for results. By using these extension methods, you can quickly and efficiently transform an object's properties into a key-value pair representation that can be easily consumed in different contexts.
/// </summary>
public static class ObjectExtensions
{
    private static readonly Lazy<ConcurrentDictionary<Type, string[]>> _mappableMemberCache = new([]);
    /// <summary>
    /// Converts an object of type T into a dictionary where the keys are the property names and the values are the corresponding property values. This method uses reflection to access the properties of the object and create a dictionary representation of it. The resulting dictionary can be used for various purposes, such as serialization, logging, or creating metadata for results. By calling this extension method on an object, you can easily obtain a dictionary representation of its properties and their values.
    /// </summary>
    /// <typeparam name="T">The type of the object to convert.</typeparam>
    /// <param name="source">The object to convert into a dictionary.</param>
    /// <returns>A dictionary containing the property names and values of the object.</returns>
    public static IReadOnlyDictionary<string, object?> ToDictionary<T>(this T source)
    {
        Dictionary<string, object?> itemValueDictionary = [];
        var type = source?.GetType() ?? typeof(T);
        var sourceAccessor = TypeAccessor.Create(type);
        var sourceTypeNames = _mappableMemberCache.Value.GetOrAdd(type, (t) => [.. sourceAccessor.GetMembers().Where(m => m.CanWrite && m.CanRead).Select(m => m.Name)]);

        foreach (var name in sourceTypeNames)
        {
            var s = sourceAccessor[source, name];
            itemValueDictionary.TryAdd(name, s);
        }

        return itemValueDictionary;
    }
}
