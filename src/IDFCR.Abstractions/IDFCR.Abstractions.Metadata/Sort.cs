namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents a static factory class for creating instances of the <see cref="ISort"/> interface, providing a convenient method for generating sorting specifications based on a specified field and order direction. The Create method takes a field name and an order direction as parameters and returns an instance of the DefaultSort record, which implements the ISort interface. This factory class simplifies the process of creating sorting specifications by abstracting away the details of the DefaultSort implementation and allowing developers to easily generate sorting criteria using a straightforward method call. By using this factory class, developers can enhance code readability and maintainability when defining sorting logic in their applications and services that utilize structured ordered requests.
/// </summary>
public static class Sort
{
    /// <summary>
    /// Creates a new instance of the <see cref="ISort"/> interface based on the specified field and order direction. This method serves as a convenient factory method for generating sorting specifications, allowing developers to easily create instances of the DefaultSort record without needing to directly instantiate it. The field parameter specifies the name of the field to sort by, while the orderDirection parameter indicates the direction of sorting (ascending or descending) using the OrderDirection enum. By using this method, developers can streamline the process of defining sorting criteria in their applications and services that utilize structured ordered requests, enhancing code readability and maintainability.
    /// </summary>
    /// <param name="field">The name of the field to sort by.</param>
    /// <param name="orderDirection">The direction of sorting (ascending or descending) using the OrderDirection enum.</param>
    /// <returns>An instance of the <see cref="ISort"/> interface representing the specified sorting criteria.</returns>
    public static ISort Create(string field, OrderDirection orderDirection)
    {
        return new DefaultSort
        {
            Field = field,
            Order = (int)orderDirection
        };
    }
}