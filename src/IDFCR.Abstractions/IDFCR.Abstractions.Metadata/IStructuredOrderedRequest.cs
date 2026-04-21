namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents a request that includes an optional ordering specification, allowing for the sorting of results based on a specified property or properties. This interface extends the <see cref="IOrderedRequest"/> interface and provides additional functionality to convert the ordering specification into a collection of <see cref="ISort"/> objects, which can be used to apply sorting logic in a more structured and type-safe manner. By implementing this interface, developers can standardize how ordering information is represented and processed across different parts of the application, enabling dynamic sorting of data based on client input while maintaining a clear and consistent approach to handling sorting specifications.
/// </summary>
public interface IStructuredOrderedRequest
{
    /// <summary>
    /// Converts the current request into an <see cref="IOrderedRequest"/> instance, which includes the ordering specification as a string that can be used by query handlers or services to apply sorting logic to data retrieval operations. The method takes an optional <see cref="OrderDirection"/> parameter that can be used to specify a default sorting direction (ascending or descending) when the <see cref="IOrderedRequest.OrderBy"/> property does not explicitly define the sorting direction for the specified properties. This allows for more flexible and user-friendly sorting behavior in scenarios where clients may not always specify the sorting direction in their requests. The conversion process involves constructing an appropriate ordering specification string based on the collection of <see cref="ISort"/> objects defined in the <see cref="Fields"/> property, ensuring that the resulting <see cref="IOrderedRequest"/> instance accurately represents the intended sorting criteria for the request.
    /// </summary>
    /// <returns>Returns an <see cref="IOrderedRequest"/> instance representing the current request's sorting specifications.</returns>
    IOrderedRequest ToOrderedRequest(OrderDirection? defaultOrderDirection);

    /// <summary>
    /// Parses the provided JSON string to populate the collection of <see cref="ISort"/> objects in the <see cref="Fields"/> property. The JSON string is expected to contain an array of sorting specifications, where each specification includes the field name and the sorting direction (e.g., ascending or descending). This method allows for dynamic parsing of sorting criteria from client input, enabling flexible and user-friendly sorting behavior in scenarios where clients may provide sorting specifications in a structured format. By implementing this method, developers can easily convert JSON-based sorting specifications into a collection of <see cref="ISort"/> objects that can be used to apply sorting logic in query handlers or services throughout the application.
    /// </summary>
    /// <param name="json">The JSON string containing the sorting specifications.</param>
    void ParseFields(string json);

    /// <summary>
    /// Gets the collection of <see cref="ISort"/> objects that represent the sorting specifications for the request. This property provides a structured representation of the sorting criteria defined in the <see cref="IOrderedRequest.OrderBy"/> property of the base interface, allowing for easier processing and application of sorting logic in query handlers or services. Each <see cref="ISort"/> object in the collection corresponds to a specific field and its associated sorting direction, enabling dynamic sorting of data based on client input while maintaining a clear and consistent approach to handling sorting specifications across different parts of the application.
    /// </summary>
    IEnumerable<ISort> Fields { get; }
}
