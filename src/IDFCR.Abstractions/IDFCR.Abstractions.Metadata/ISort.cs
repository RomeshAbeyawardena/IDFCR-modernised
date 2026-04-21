namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents a sorting specification for a particular field, including the field name, the order of sorting, and the direction of sorting (ascending or descending). This interface is typically used in conjunction with ordered requests to define how results should be sorted based on specific properties. The <see cref="Field"/> property indicates the name of the field to sort by, while the <see cref="Order"/> property specifies the order of sorting (e.g., 1 for ascending, -1 for descending). The <see cref="Direction"/> property provides a convenient way to determine the sorting direction based on the value of the <see cref="Order"/> property, allowing for more intuitive handling of sorting logic in applications that support dynamic ordering of data.
/// </summary>
public interface ISort
{
    /// <summary>
    /// Gets the name of the field to sort by.
    /// </summary>
    string Field { get; }
    /// <summary>
    /// Gets the order of sorting for the specified field, where a positive value (e.g., 1) indicates ascending order, a negative value (e.g., -1) indicates descending order, and zero or null can indicate no specific order. This property allows for flexible specification of sorting preferences for different fields in a request, enabling dynamic sorting of results based on client input or application logic.
    /// </summary>
    int Order { get; }
    /// <summary>
    /// Gets the direction of sorting for the specified field, based on the value of the <see cref="Order"/> property. This property provides a convenient way to determine whether the sorting is ascending, descending, or not specified, allowing for more intuitive handling of sorting logic in applications that support dynamic ordering of data.
    /// </summary>  
    OrderDirection Direction => (OrderDirection)Order;
}
