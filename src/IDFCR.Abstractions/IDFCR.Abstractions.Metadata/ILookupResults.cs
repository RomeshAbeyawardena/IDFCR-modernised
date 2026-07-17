namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents the result of a lookup operation that returns a collection of results, including the provider type and the result object.
/// </summary>
/// <typeparam name="T">The type of the elements in the result collection.</typeparam>
public interface ILookupResults<T> : ILookupResult<IEnumerable<T>>
{
    /// <summary>
    /// Gets the results of the lookup operation, which may be empty if no results were found.
    /// </summary>
    IEnumerable<T> Results { get; }
}