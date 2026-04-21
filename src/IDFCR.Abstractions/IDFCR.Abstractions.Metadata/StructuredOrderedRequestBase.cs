using IDFCR.Abstractions.Metadata.Extensions;
using System.Text;

namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents a base implementation of the <see cref="IStructuredOrderedRequest"/> interface, providing a default implementation for the <see cref="Fields"/> property and the <see cref="ToOrderedRequest"/> method. This abstract record serves as a foundation for request models that require structured ordering specifications, allowing developers to easily create concrete implementations by inheriting from this base class. The <see cref="ToOrderedRequest"/> method processes the collection of <see cref="ISort"/> objects defined in the <see cref="Fields"/> property and constructs an appropriate ordering specification string that can be used by query handlers or services to apply sorting logic to data retrieval operations. By using this base class, developers
/// </summary>
public abstract record StructuredOrderedRequestBase : IStructuredOrderedRequest
{

    /// <inheritdoc />
    public IEnumerable<ISort> Fields { get; init; } = [];

    /// <inheritdoc />
    public IOrderedRequest ToOrderedRequest(OrderDirection? defaultOrderDirection)
    {
        StringBuilder orderByStringBuilder = new();
        string delimiter = string.Empty;
        foreach(var field in Fields)
        {
            orderByStringBuilder.Append($"{delimiter}{field.Field} {field.Direction.ToDirectionString()}");
            if (string.IsNullOrEmpty(delimiter))
            {
                delimiter = ", ";
            }
        }

        return new DefaultOrderedRequest { 
            OrderBy = orderByStringBuilder.ToString(),
            DefaultOrderDirection = defaultOrderDirection
        };
    }
}
