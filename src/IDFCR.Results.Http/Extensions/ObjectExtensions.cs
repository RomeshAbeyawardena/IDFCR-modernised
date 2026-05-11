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
    /// Determines whether the specified object is considered a primitive type. This method checks if the object is of a primitive type such as bool, string, Guid, decimal, int, float, double, DateTime, or DateTimeOffset. If the object is of any of these types, it returns true; otherwise, it returns false. This method can be useful for determining if an object can be directly represented as a simple value or if it requires more complex handling when converting to a dictionary or performing other operations.
    /// </summary>
    /// <param name="source">The object to check.</param>
    /// <returns>True if the object is a primitive type; otherwise, false.</returns>
    public static bool IsPrimitive(object? source)
    {
        return source is bool 
            || source is string 
            || source is Guid 
            || source is decimal 
            || source is int
            || source is float
            || source is double
            || source is DateTime
            || source is DateTimeOffset;
    }

    /// <summary>
    /// Determines whether the specified type is considered a complex type. This method checks if the type is a generic type and if it implements certain collection interfaces.
    /// </summary>
    /// <param name="sourceType">The type to check.</param>
    /// <returns>True if the type is a complex type; otherwise, false.</returns>
    public static bool IsComplexType(Type sourceType)
    {
        if (sourceType.IsGenericType)
        {
            var genericArguments = sourceType.GetGenericArguments();

            if (genericArguments.Length == 1)
            {
                return typeof(IEnumerable<>).MakeGenericType(genericArguments).IsAssignableFrom(sourceType)
                    || typeof(IReadOnlyCollection<>).MakeGenericType(genericArguments).IsAssignableFrom(sourceType)
                    || typeof(IReadOnlyList<>).MakeGenericType(genericArguments).IsAssignableFrom(sourceType)
                    || typeof(List<>).MakeGenericType(genericArguments).IsAssignableFrom(sourceType)
                    || typeof(Queue<>).MakeGenericType(genericArguments).IsAssignableFrom(sourceType)
                    || typeof(Stack<>).MakeGenericType(genericArguments).IsAssignableFrom(sourceType);
            }
            else if (genericArguments.Length == 2)
            {
                return  typeof(IDictionary<,>).MakeGenericType(genericArguments).IsAssignableFrom(sourceType)
                    || typeof(Dictionary<,>).MakeGenericType(genericArguments).IsAssignableFrom(sourceType)
                    || typeof(ConcurrentDictionary<,>).MakeGenericType(genericArguments).IsAssignableFrom(sourceType)
                    || typeof(Tuple<,>).MakeGenericType(genericArguments).IsAssignableFrom(sourceType)
                    || typeof(ValueTuple<,>).MakeGenericType(genericArguments).IsAssignableFrom(sourceType);
            }
            else if (genericArguments.Length == 3)
            {
                return typeof(Tuple<,,>).MakeGenericType(genericArguments).IsAssignableFrom(sourceType)
                    || typeof(ValueTuple<,,>).MakeGenericType(genericArguments).IsAssignableFrom(sourceType);
            }
            else if (genericArguments.Length == 4)
            {
                return typeof(Tuple<,,,>).MakeGenericType(genericArguments).IsAssignableFrom(sourceType)
                    || typeof(ValueTuple<,,,>).MakeGenericType(genericArguments).IsAssignableFrom(sourceType);
            }
            else if (genericArguments.Length == 5)
            {
                return typeof(Tuple<,,,,>).MakeGenericType(genericArguments).IsAssignableFrom(sourceType)
                    || typeof(ValueTuple<,,,,>).MakeGenericType(genericArguments).IsAssignableFrom(sourceType);
            }
        }

        return false;
    }

    /// <summary>
    /// Converts an object of type T into a dictionary where the keys are the property names and the values are the corresponding property values. This method uses reflection to access the properties of the object and create a dictionary representation of it. The resulting dictionary can be used for various purposes, such as serialization, logging, or creating metadata for results. By calling this extension method on an object, you can easily obtain a dictionary representation of its properties and their values.
    /// </summary>
    /// <typeparam name="T">The type of the object to convert.</typeparam>
    /// <param name="source">The object to convert into a dictionary.</param>
    /// <returns>A dictionary containing the property names and values of the object.</returns>
    public static IReadOnlyDictionary<string, object?> ToDictionary<T>(this T source)
    {
        if (source is null)
        {
            return new Dictionary<string, object?>();
        }

        var type = source?.GetType() ?? typeof(T);

        if (IsPrimitive(source) || IsComplexType(type))
        {
            return new Dictionary<string, object?> { ["Value"] = source };
        }

        Dictionary<string, object?> itemValueDictionary = [];
        
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
