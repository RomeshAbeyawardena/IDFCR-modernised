namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents the direction of ordering for sorting operations, indicating whether the sorting should be in ascending or descending order. This enum is typically used in conjunction with sorting and ordering functionalities in APIs, query builders, or any context where the order of data needs to be specified. The values of this enum allow for clear and concise representation of the desired sorting direction, enhancing readability and maintainability of code that involves sorting logic.
/// </summary>
public enum OrderDirection
{
    /// <summary>
    /// Ascending order, where values are sorted from lowest to highest (e.g., A to Z for strings, 0 to 9 for numbers). This is the default sorting direction when no specific order is specified.
    /// </summary>
    Ascending,
    /// <summary>
    /// Descending order, where values are sorted from highest to lowest (e.g., Z to A for strings, 9 to 0 for numbers). This is used when the sorting needs to be in reverse order compared to ascending.
    /// </summary>
    Descending
}

/// <summary>
/// Represents a request that includes an optional ordering specification, allowing for the sorting of results based on a specified property or properties. The <see cref="OrderBy"/> property is a string that can contain one or more comma-separated values indicating the properties to sort by and their respective sorting directions (e.g., "Name desc, Age"). This interface is commonly implemented by request models in APIs or query handlers that need to support dynamic sorting of data based on client input. By implementing this interface, developers can standardize how ordering information is represented and processed across different parts of the application.
/// </summary>
public interface IOrderedRequest
{
    /// <summary>
    /// Gets the ordering specification for the request, which is a string that can contain one or more comma-separated values indicating the properties to sort by and their respective sorting directions (e.g., "Name desc, Age"). This property allows clients to specify how they want the results to be ordered when making a request, enabling dynamic sorting of data based on client input. The format of the string should be consistent with the expected syntax for specifying sorting criteria, and it can be processed by query handlers or services to apply the appropriate ordering to the data retrieval operations.
    /// </summary>
    string? OrderBy { get; }
    /// <summary>
    /// Gets the default order direction for the request, which is an optional value of the <see cref="OrderDirection"/> enum. This property can be used to specify a default sorting direction (ascending or descending) when the <see cref="OrderBy"/> property does not explicitly define the sorting direction for the specified properties. If this property is not set, it can be assumed that the default sorting direction is ascending. This allows for more flexible and user-friendly sorting behavior in scenarios where clients may not always specify the sorting direction in their requests.
    /// </summary>
    OrderDirection? DefaultOrderDirection { get; }
}
