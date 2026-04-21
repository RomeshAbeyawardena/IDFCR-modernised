namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents a default implementation of the <see cref="ISort"/> interface, providing properties for the field to sort by and the order direction. This record serves as a simple data structure for defining sorting specifications in requests that require structured ordering. The Field property specifies the name of the field to sort by, while the Order property indicates the direction of sorting (ascending or descending) using an integer value. This implementation can be used as a default or base class for more complex sorting requirements, allowing developers to easily create instances of sorting specifications without needing to implement the ISort interface directly. By using this default implementation, developers can streamline the process of defining sorting criteria in their applications and services that utilize structured ordered requests.
/// </summary>
public record DefaultSort : ISort
{
    /// <inheritdoc />
    public string Field { get; } = null!;
    /// <inheritdoc />
    public int Order { get; } = 0;
}
