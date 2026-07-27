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
    /// <param name="additionalManipulation">An optional function to apply additional manipulation to the string after normalization.</param>
    /// <returns>The normalized string, or null if the input is null or whitespace.</returns>
    public static string? NormalizeString(this string? value, Func<string?, string?>? additionalManipulation = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        return additionalManipulation?.Invoke(normalized) ?? normalized;
    }
}
