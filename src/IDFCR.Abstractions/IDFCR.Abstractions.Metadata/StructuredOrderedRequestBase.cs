using IDFCR.Abstractions.Metadata.Extensions;
using System.Text;
using System.Text.Json;

namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents a base implementation of the <see cref="IStructuredOrderedRequest"/> interface, providing a default implementation for the <see cref="Fields"/> property and the <see cref="ToOrderedRequest"/> method. This abstract record serves as a foundation for request models that require structured ordering specifications, allowing developers to easily create concrete implementations by inheriting from this base class. The <see cref="ToOrderedRequest"/> method processes the collection of <see cref="ISort"/> objects defined in the <see cref="Fields"/> property and constructs an appropriate ordering specification string that can be used by query handlers or services to apply sorting logic to data retrieval operations. By using this base class, developers
/// </summary>
public abstract record StructuredOrderedRequestBase : IStructuredOrderedRequest
{
    private IEnumerable<ISort> _fields = [];

    /// <summary>
    /// Sets the collection of <see cref="ISort"/> objects that represent the sorting specifications for the request. This property is used internally to populate the <see cref="Fields"/> property and should not be exposed publicly, as it is intended for use within the implementation of the <see cref="ParseFields"/> method to convert JSON-based sorting specifications into a structured format that can be easily processed by query handlers or services throughout the application.
    /// </summary>
    protected IEnumerable<ISort> SortedFields { set => _fields = value; }

    /// <inheritdoc />
    public IEnumerable<ISort> Fields { get => _fields; init => _fields = value; }

    /// <inheritdoc />
    public void ParseFields(string json)
    {
        SortedFields = JsonSerializer.Deserialize<IEnumerable<DefaultSort>>(json) ?? [];
    }

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
