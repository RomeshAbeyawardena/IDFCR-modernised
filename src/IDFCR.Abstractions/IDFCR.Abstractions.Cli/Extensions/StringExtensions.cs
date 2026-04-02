using System.Text.Json;

namespace IDFCR.Abstractions.Cli.Extensions;

/// <summary>
/// Defines extension methods for string manipulation, including JSON serialization and string length limiting.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Serializes an object to a JSON string using the specified <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="value">The object to serialize.</param>
    /// <param name="options">The JSON serializer options to use.</param>
    /// <returns>A JSON string representation of the object.</returns>
    public static string Jsonify<T>(this T? value, JsonSerializerOptions options)
    {
        if(value is null)
        {
            return "{}";
        }

        return JsonSerializer.Serialize(value, options);
    }

    /// <summary>
    /// Limits the length of a string to a specified number of characters. If the string exceeds the limit, it truncates the string and appends an ellipsis ("..."). If the string is shorter than the limit, it pads the string with spaces to reach the limit. If the limit is less than 4, it returns just an ellipsis for any non-empty string. If the input string is null or whitespace, it returns the original value.
    /// </summary>
    /// <param name="value">The string to limit.</param>
    /// <param name="limit">The maximum length of the string.</param>
    /// <returns>The limited string.</returns>
    public static string? Limit(this string? value, int limit)
    {
        if (string.IsNullOrWhiteSpace(value) || limit == 0)
        {
            return value;
        }

        if (limit < 4)
        {
            return "...";
        }

        if (value.Length > limit)
        {
            return string.Concat(value.AsSpan(0, limit - 3), "..."); 
        }

        if (value.Length < limit)
        {
            var len = limit - value.Length;
            var r = string.Concat(value, new string(' ', len));
            return r;
        }

        return value;
    }
}
