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
    private readonly List<string> _supportedFields = [];

    /// <summary>
    /// Gets the list of supported fields for sorting. This property is used internally to validate the fields specified in the request and should not be modified directly. Derived classes can add supported fields to this list to enable sorting on specific properties.
    /// </summary>
    protected List<string> SupportedFields => _supportedFields;

    /// <summary>
    /// Validates the collection of <see cref="ISort"/> objects against the list of supported fields. This method checks if all fields specified in the sorting specifications are included in the list of supported fields. If the list of supported fields is empty, it is assumed that all fields are valid. This validation ensures that only valid and supported fields are used for sorting, preventing potential errors or issues when processing the sorting specifications in query handlers or services.
    /// </summary>
    /// <param name="fields">The collection of fields to validate.</param>
    /// <returns>True if all fields are valid and supported; otherwise, false.</returns>
    protected virtual bool ValidateFields(IEnumerable<ISort> fields)
    {
        return _supportedFields.Count == 0 || fields.All(f => _supportedFields.Contains(f.Field, StringComparer.InvariantCultureIgnoreCase));
    }

    /// <summary>
    /// Sets the collection of <see cref="ISort"/> objects that represent the sorting specifications for the request. This property is used internally to populate the <see cref="Fields"/> property and should not be exposed publicly, as it is intended for use within the implementation of the <see cref="ParseFields"/> method to convert JSON-based sorting specifications into a structured format that can be easily processed by query handlers or services throughout the application.
    /// </summary>
    protected IEnumerable<ISort> SortedFields { 
        set 
        {
            if (!ValidateFields(value))
            {
                throw new FieldValidationException("One or several requested fields are not supported.");
            }

            _fields = value;
        }
    }

    /// <inheritdoc />
    public IEnumerable<ISort> Fields { get => _fields; init => SortedFields = value; }

    /// <inheritdoc />
    public void ParseFields(string json)
    {
        SortedFields = JsonSerializer.Deserialize<IEnumerable<DefaultSort>>(json, JsonSerializerOptions.Web) ?? [];
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
