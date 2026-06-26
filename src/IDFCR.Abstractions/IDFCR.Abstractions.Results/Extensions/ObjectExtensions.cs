using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace IDFCR.Abstractions.Results.Extensions;

/// <summary>
/// Defines an enumeration to specify additional conditions for type checking in the IsOf extension method. The And enum provides options to indicate whether the type check should also consider whether the value is not equal to the default value of the specified type. This allows for more flexible and precise type checking when using the IsOf method, enabling developers to easily check for both type compatibility and non-default values in a single method call.
/// </summary>
public enum And
{
    /// <summary>
    /// Defines a value indicating that the type check should not consider whether the value is equal to the default value of the specified type. When this option is used, the IsOf method will only check if the object is of the specified type, without any additional conditions related to the value's default state.
    /// </summary>
    [Obsolete("This is for internal use. Use `And.NotDefault` instead.")]
    None,
    /// <summary>
    /// Defines a value indicating that the type check should also consider whether the value is not equal to the default value of the specified type. When this option is used, the IsOf method will check if the object is of the specified type and also ensure that the value is not equal to the default value for that type (e.g., null for reference types, 0 for numeric types, etc.). This allows for a more comprehensive type check that ensures both type compatibility and non-default values.
    /// </summary>
    NotDefault
}


/// <summary>
/// Defines extension methods for the Object class, providing additional functionality to determine if an object is of a specific type and to chain unit results together. These extension methods allow for more convenient and expressive code when working with objects and unit results, enabling developers to easily check types and create complex result chains while preserving the information and status of each individual result in the chain.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Determines whether the specified object is of type Guid. This method checks if the provided object can be cast to a Guid. If the object is of type Guid, it returns true and outputs the value; otherwise, it returns false and outputs null. Additionally, if the object is not of type Guid but can be parsed as a valid Guid string representation, it will also return true and output the parsed Guid value.
    /// </summary>
    /// <param name="source">The object to check.</param>
    /// <param name="value">The output value if the object is of type Guid or can be parsed as a valid Guid string.</param>
    /// <returns>True if the object is of type Guid or can be parsed as a valid Guid string; otherwise, false.</returns>
    public static bool IsOfGuid(this object? source, [NotNullWhen(true)] out Guid? value)
    {
        if (IsOf(source, out value))
        {
            return true;
        }

        if (Guid.TryParse(source?.ToString(), out var guid))
        {
            value = guid;
            return true;
        }

        return false;
    }

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
    /// <param name="and">Specifies additional conditions to check for the object.</param>
    /// <param name="value">The output value if the object is of the specified type and not the default value.</param>
    /// <returns>True if the object is of the specified type and not the default value; otherwise, false.</returns>
    public static bool IsOf<T>(this object? source, And and, [NotNullWhen(true)] out T? value)
     where T : struct
    {
        value = null;

        var comparer = EqualityComparer<T>.Default;
#pragma warning disable CS0618 // Type or member is obsolete
        // Check if it's of type T, and then check that the underlying raw value is not default
        if (source.IsOf<T>(out var nullableVal) && (and == And.None || !comparer.Equals(nullableVal.Value, default)))
        {
            value = nullableVal;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the specified object is of type Guid and not equal to the default value of Guid. This method checks if the provided object can be cast to a Guid and also ensures that the value is not equal to the default value for Guid (Guid.Empty). If both conditions are met, it returns true and outputs the value; otherwise, it returns false and outputs null.
    /// </summary>
    /// <param name="source">The object to check.</param>
    /// <param name="and">Specifies additional conditions to check for the object.</param>
    /// <param name="value">The output value if the object is of type Guid and not the default value.</param>
    /// <returns>True if the object is of type Guid and not the default value; otherwise, false.</returns>
    public static bool IsOfGuid(this object? source, And and, [NotNullWhen(true)] out Guid? value)
    {
        if (IsOf(source, and, out value))
        {
            return true;
        }

        if (Guid.TryParse(source?.ToString(), out var guid) && (and == And.None || guid != default))
        {
            value = guid;
            return true;
        }
#pragma warning restore CS0618 // Type or member is obsolete
        return false;
    }
}
