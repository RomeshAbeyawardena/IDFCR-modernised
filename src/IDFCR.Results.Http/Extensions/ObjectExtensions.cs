using FastMember;
using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json;

namespace IDFCR.Results.Http.Extensions;

/// <summary>
/// Defines extension methods for converting objects into dictionaries. These extension methods allow you to easily convert an object's properties into a dictionary format, which can be useful for various purposes such as serialization, logging, or creating metadata for results. By using these extension methods, you can quickly and efficiently transform an object's properties into a key-value pair representation that can be easily consumed in different contexts.
/// </summary>
public static class ObjectExtensions
{
    private static readonly Lazy<ConcurrentDictionary<Type, string[]>> _mappableMemberCache = new([]);

    /// <summary>
    /// Determines whether the specified object is considered a primitive/scalar type.
    /// </summary>
    /// <param name="source">The object to check.</param>
    /// <returns>True if the object is a primitive/scalar type; otherwise, false.</returns>
    public static bool IsPrimitive(object? source)
    {
        return source is not null && IsPrimitiveType(source.GetType());
    }

    /// <summary>
    /// Determines whether the specified type is considered a complex collection/container type.
    /// </summary>
    /// <param name="sourceType">The type to check.</param>
    /// <returns>True if the type is a complex collection/container type; otherwise, false.</returns>
    public static bool IsComplexType(Type sourceType)
    {
        if (sourceType == typeof(string) || IsPrimitiveType(sourceType))
        {
            return false;
        }

        if (sourceType.IsArray)
        {
            return true;
        }

        if (typeof(IDictionary).IsAssignableFrom(sourceType))
        {
            return true;
        }

        if (ImplementsGenericInterface(sourceType, typeof(IDictionary<,>)))
        {
            return true;
        }

        if (IsTupleType(sourceType))
        {
            return true;
        }

        return typeof(IEnumerable).IsAssignableFrom(sourceType)
            || ImplementsGenericInterface(sourceType, typeof(IEnumerable<>));
    }

    /// <summary>
    /// Converts an object of type T into a dictionary where the keys are the property names and the values are the corresponding property values. This method uses reflection to access the properties of the object and create a dictionary representation of it. The resulting dictionary can be used for various purposes, such as serialization, logging, or creating metadata for results. By calling this extension method on an object, you can easily obtain a dictionary representation of its properties and their values.
    /// </summary>
    /// <typeparam name="T">The type of the object to convert.</typeparam>
    /// <param name="source">The object to convert into a dictionary.</param>
    /// <param name="singularKey">An optional key to use for the dictionary entry if the object is a primitive or complex type.</param>
    /// <returns>A dictionary containing the property names and values of the object.</returns>
    public static IReadOnlyDictionary<string, object?> ToDictionary<T>(this T source, string? singularKey = null)
    {
        if (source is null)
        {
            return new Dictionary<string, object?>();
        }

        var type = source.GetType();

        if (IsPrimitive(source) || IsComplexType(type))
        {
            return new Dictionary<string, object?> { [singularKey ?? type.Name] = source };
        }

        Dictionary<string, object?> itemValueDictionary = [];

        var sourceAccessor = TypeAccessor.Create(type);
        var sourceTypeNames = _mappableMemberCache.Value.GetOrAdd(
            type,
            _ => [.. sourceAccessor.GetMembers().Where(m => m.CanRead).Select(m => m.Name)]
        );

        foreach (var name in sourceTypeNames)
        {
            try
            {
                var s = sourceAccessor[source, name];
                itemValueDictionary.TryAdd(JsonNamingPolicy.CamelCase.ConvertName(name), s);
            }
            catch
            {
                
            }
        }

        return itemValueDictionary;
    }

    private static bool IsPrimitiveType(Type type)
    {
        var actualType = Nullable.GetUnderlyingType(type) ?? type;

        if (actualType.IsEnum || actualType.IsPrimitive)
        {
            return true;
        }

        return actualType == typeof(string)
            || actualType == typeof(decimal)
            || actualType == typeof(Guid)
            || actualType == typeof(DateTime)
            || actualType == typeof(DateTimeOffset)
            || actualType == typeof(TimeSpan)
            || actualType == typeof(DateOnly)
            || actualType == typeof(TimeOnly);
    }

    private static bool ImplementsGenericInterface(Type sourceType, Type genericInterfaceType)
    {
        return sourceType.IsGenericType && sourceType.GetGenericTypeDefinition() == genericInterfaceType
            || sourceType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterfaceType);
    }

    private static bool IsTupleType(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        var genericTypeDefinition = type.GetGenericTypeDefinition();

        return genericTypeDefinition == typeof(Tuple<>)
            || genericTypeDefinition == typeof(Tuple<,>)
            || genericTypeDefinition == typeof(Tuple<,,>)
            || genericTypeDefinition == typeof(Tuple<,,,>)
            || genericTypeDefinition == typeof(Tuple<,,,,>)
            || genericTypeDefinition == typeof(Tuple<,,,,,>)
            || genericTypeDefinition == typeof(Tuple<,,,,,,>)
            || genericTypeDefinition == typeof(Tuple<,,,,,,,>)
            || genericTypeDefinition == typeof(ValueTuple<>)
            || genericTypeDefinition == typeof(ValueTuple<,>)
            || genericTypeDefinition == typeof(ValueTuple<,,>)
            || genericTypeDefinition == typeof(ValueTuple<,,,>)
            || genericTypeDefinition == typeof(ValueTuple<,,,,>)
            || genericTypeDefinition == typeof(ValueTuple<,,,,,>)
            || genericTypeDefinition == typeof(ValueTuple<,,,,,,>)
            || genericTypeDefinition == typeof(ValueTuple<,,,,,,,>);
    }
}
