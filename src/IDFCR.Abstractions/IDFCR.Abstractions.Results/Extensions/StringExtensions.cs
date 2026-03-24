using System.Globalization;
using System.Text.RegularExpressions;

namespace IDFCR.Abstractions.Results.Extensions;

/// <summary>
/// String utility extensions used by the result and exception helpers.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Normalizes spacing and title-cases the supplied value.
    /// </summary>
    public static string NormaliseName(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;
        value = Regex.Replace(value, @"\s+", " ").Trim();
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLowerInvariant());
    }
    /// <summary>
    /// Pads, trims, or elides the value to a fixed length.
    /// </summary>
    public static string FixedLength(this string value, int length)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new string(' ', length);
        }

        if (value.Length > length && length - 3 > 0)
        {
            return string.Concat(value.AsSpan(0, length - 3).Trim(), "...");
        }

        if (value.Length < length)
        {
            var spacesToAdd = length - value.Length;
            return string.Concat(value, new string(' ', spacesToAdd));
        }

        return value;
    }

    /// <summary>
    /// Converts the value to kebab-case.
    /// </summary>
    public static string ToKebabCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        var result = new List<char>();

        for (int i = 0; i < value.Length; i++)
        {
            char current = value[i];
            char? prev = i > 0 ? value[i - 1] : (char?)null;
            char? next = i < value.Length - 1 ? value[i + 1] : (char?)null;

            if (current == '_' || current == ' ')
            {
                if (result.Count > 0 && result[^1] != '-') result.Add('-');
                continue;
            }

            if (char.IsUpper(current))
            {
                bool isAcronymEnd = next.HasValue && char.IsLower(next.Value);
                bool isWordBreak = prev.HasValue && !char.IsUpper(prev.Value);

                if ((isAcronymEnd || isWordBreak) && result.Count > 0 && result[^1] != '-')
                    result.Add('-');

                result.Add(char.ToLower(current));
            }
            else
            {
                if (prev.HasValue && (
                    (char.IsDigit(prev.Value) && char.IsLetter(current)) ||
                    (char.IsLetter(prev.Value) && char.IsDigit(current))))
                {
                    result.Add('-');
                }

                result.Add(char.ToLower(current));
            }
        }

        return new string([.. result]).Trim('-');
    }

    /// <summary>
    /// Converts the value to camel case while preserving acronym casing.
    /// </summary>
    public static string ToCamelCasePreservingAcronyms(this string input)
    {
        var upperCaseWordRegex = new Regex("[A-Z]+");

        var result = new List<char>(input);
        bool isFirstChar = true;
        var matches = upperCaseWordRegex.Matches(input);
        foreach (Match match in matches)
        {
            if (isFirstChar)
            {
                result[match.Index] = char.ToLowerInvariant(match.Value[0]);
                isFirstChar = false;
            }

            if (match.Index == 0 && match.Length > 1 && IsAllUpper(match.Value))
            {
                result[match.Index] = char.ToLowerInvariant(result[match.Index]);
                for (int j = 1; j < match.Length - 1; j++)
                {
                    result[match.Index + j] = char.ToLowerInvariant(result[match.Index + j]);
                }
            }

            var nextCharIndex = match.Index + 1;

            if (nextCharIndex < result.Count)
            {
                if (char.IsUpper(result[nextCharIndex]))
                {
                    continue;
                }
            }
        }

        return new string([.. result]) ?? string.Empty;
    }

    private static bool IsAllUpper(string word) =>
        word.All(char.IsLetter) && word.Equals(word, StringComparison.InvariantCultureIgnoreCase);

    /// <summary>
    /// Replaces all occurrences of the supplied values.
    /// </summary>
    public static string ReplaceAll(this string value, string newValue, params string[] values)
    {
        return ReplaceAll(value, newValue, default, values);
    }

    /// <summary>
    /// Replaces all occurrences of the supplied values using the specified comparison rules.
    /// </summary>
    public static string ReplaceAll(this string value, string newValue, StringComparison stringComparison, params string[] values)
    {
        if (values == null || values.Length == 0)
            return value;
        foreach (var item in values)
        {
            value = value.Replace(item, newValue, stringComparison);
        }
        return value;
    }

    /// <summary>
    /// Prepends a URL segment using a forward slash separator.
    /// </summary>
    public static string PrependUrl(this string value, string source)
    {
        return Prepend(value, source, '/');
    }
    /// <summary>
    /// Prepends a string using the supplied separator.
    /// </summary>
    public static string Prepend(this string value, string source, char? separator)
    {
        return $"{source}{separator}{value}";
    }
}
