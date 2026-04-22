namespace IDFCR.Abstractions.Metadata.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="OrderDirection"/> enum, providing functionality to convert the enum values to their corresponding string representations ("asc" for ascending and "desc" for descending). This is useful for scenarios where the order direction needs to be represented as a string, such as in query parameters or sorting specifications in APIs. The extension method ensures a consistent and centralized way to handle the conversion of order directions throughout the application.
/// </summary>
public static class OrderDirectionExtensions
{
    /// <summary>
    /// Gets the string representation of the specified <see cref="OrderDirection"/> value. The method returns "asc" for <see cref="OrderDirection.Ascending"/> and "desc" for <see cref="OrderDirection.Descending"/>. If an invalid value is provided, an <see cref="ArgumentOutOfRangeException"/> is thrown. This method is essential for converting enum values to their corresponding string formats, which are commonly used in sorting and ordering contexts in APIs and data processing.
    /// </summary>
    /// <param name="direction">The <see cref="OrderDirection"/> value to convert to a string representation.</param>
    /// <returns>A string representation of the specified <see cref="OrderDirection"/> value ("asc" or "desc").</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an invalid <see cref="OrderDirection"/> value is provided.</exception>
    public static string ToDirectionString(this OrderDirection direction)
    {
        return direction switch
        {
            OrderDirection.Ascending => "asc",
            OrderDirection.Descending => "desc",
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}
