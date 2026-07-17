using System.ComponentModel;

namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents the result of a lookup operation, including the provider type and the result object.
/// </summary>
/// <param name="Provider">The type of the provider that performed the lookup operation.</param>
/// <param name="Result">The result of the lookup operation, which may be null if no result was found.</param>
public record LookupResult(Type Provider, object? Result) : ILookupResult;

/// <summary>
/// Represents the result of a lookup operation with a specific result type, including the provider type and the result object.
/// </summary>
/// <typeparam name="T">The type of the result object.</typeparam>
/// <param name="Provider">The type of the provider that performed the lookup operation.</param>
/// <param name="Result">The result of the lookup operation, which may be null if no result was found.</param>
public record LookupResult<T>(Type Provider, T? Result) : LookupResultBase(Provider, Result), ILookupResult<T>;

/// <summary>
/// Represents the result of a lookup operation that returns a collection of results, including the provider type and the result object.
/// </summary>
/// <typeparam name="T">The type of the result objects.</typeparam>
public record LookupResults<T>(IEnumerable<ILookupResult<T>> Results) : ILookupResults<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LookupResults{T}"/> class with the specified lookup results.
    /// </summary>
    /// <param name="lookupResults">The lookup results to initialize the instance with.</param>
    public LookupResults(ILookupResults<T> lookupResults)
        : this(lookupResults.Results)
    {
        
    }
}