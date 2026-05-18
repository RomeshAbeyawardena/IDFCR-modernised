using System.Diagnostics.CodeAnalysis;

namespace IDFCR.Abstractions.Results.Extensions;

/// <summary>
/// Defines extension methods for the Object class, providing additional functionality to determine if an object is of a specific type and to chain unit results together. These extension methods allow for more convenient and expressive code when working with objects and unit results, enabling developers to easily check types and create complex result chains while preserving the information and status of each individual result in the chain.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Determines whether the specified object is of a given type. This method checks if the provided object can be cast to the specified type T. If the object is of the specified type, it returns true and outputs the value; otherwise, it returns false and outputs null.
    /// </summary>
    /// <typeparam name="T">The type to check against.</typeparam>
    /// <param name="source">The object to check.</param>
    /// <param name="value">The output value if the object is of the specified type.</param>
    /// <returns>True if the object is of the specified type; otherwise, false.</returns>
    public static bool IsOf<T>(this object? source, [NotNullWhen(true)] out T? value)
        where T : struct
    {
        value = null;
        
        if (source is null)
        {
            return false;
        }

        if (source is T val)
        {
            value = val;
            return true;
        }

        return false;
    }
    /// <summary>
    /// Determines whether the specified object is of a given type and not equal to the default value of that type. This method checks if the provided object can be cast to the specified type T and also ensures that the value is not equal to the default value for that type (e.g., null for reference types, 0 for numeric types, etc.). If both conditions are met, it returns true and outputs the value; otherwise, it returns false and outputs the default value of type T.
    /// </summary>
    /// <typeparam name="T">The type to check against.</typeparam>
    /// <param name="source">The object to check.</param>
    /// <param name="value">The output value if the object is of the specified type and not the default value.</param>
    /// <returns>True if the object is of the specified type and not the default value; otherwise, false.</returns>
    public static bool IsOfAndNotDefault<T>(this object? source, [NotNullWhen(true)] out T? value)
     where T : struct
    {
        value = null;

        var comparer = EqualityComparer<T>.Default;

        // Check if it's of type T, and then check that the underlying raw value is not default
        if (source.IsOf<T>(out var nullableVal) && !comparer.Equals(nullableVal.Value, default))
        {
            value = nullableVal;
            return true;
        }

        return false;
    }
}
