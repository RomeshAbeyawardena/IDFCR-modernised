namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents the direction of ordering for sorting operations, indicating whether the sorting should be in ascending or descending order. This enum is typically used in conjunction with sorting and ordering functionalities in APIs, query builders, or any context where the order of data needs to be specified. The values of this enum allow for clear and concise representation of the desired sorting direction, enhancing readability and maintainability of code that involves sorting logic.
/// </summary>
public enum OrderDirection
{
    /// <summary>
    /// Ascending order, where values are sorted from lowest to highest (e.g., A to Z for strings, 0 to 9 for numbers). This is the default sorting direction when no specific order is specified.
    /// </summary>
    Ascending = 1,

    /// <summary>
    /// No specific order, indicating that the sorting direction is not defined or should not be applied. This can be used in scenarios where the order of results is not important or when the sorting direction is determined by other factors (e.g., default database ordering).
    /// </summary>
    None = 0,

    /// <summary>
    /// Descending order, where values are sorted from highest to lowest (e.g., Z to A for strings, 9 to 0 for numbers). This is used when the sorting needs to be in reverse order compared to ascending.
    /// </summary>
    Descending = -1
}
