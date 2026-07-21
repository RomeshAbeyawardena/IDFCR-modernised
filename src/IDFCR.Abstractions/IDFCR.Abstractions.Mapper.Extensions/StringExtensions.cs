namespace IDFCR.Abstractions.Mapper.Extensions;

/// <summary>
/// Defines extension methods for string manipulation and normalization.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Normalizes a string by trimming whitespace.
    /// </summary>
    /// <param name="value">The string to normalize.</param>
    /// <returns>The normalized string, or null if the input is null or whitespace.</returns>
    public static string? NormalizeString(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }
}
