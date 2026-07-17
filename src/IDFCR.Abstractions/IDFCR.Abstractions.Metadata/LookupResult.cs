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
public record LookupResults<T> : LookupResultBase<IEnumerable<T>>, ILookupResults<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LookupResults{T}"/> class with the specified provider type and result objects.
    /// </summary>
    /// <param name="provider">The type of the provider that performed the lookup operation.</param>
    /// <param name="results">The results of the lookup operation, which may be null if no results were found.</param>
    public LookupResults(Type provider, IEnumerable<T> results) : base(provider, results) { }

    /// <summary>
    /// Gets the results of the lookup operation, which may be empty if no results were found.
    /// </summary>
    public IEnumerable<T> Results => Result ?? [];

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override IEnumerable<T>? Result => base.Result;
}