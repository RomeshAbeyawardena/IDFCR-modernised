using IDFCR.Abstractions.Metadata.Lookups;
using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Results.Extensions;

namespace IDFCR.Abstractions.Lookups;
/// <summary>
/// Represents the result of a lookup operation that includes an optional unit result.S
/// </summary>
public interface ILookupUnitResult : ILookupResult
{
    /// <summary>
    /// Gets an optional unit result associated with the lookup operation. This property may be null if a unit result wasn't used by the Lookup.
    /// </summary>
    IUnitResult? UnitResult { get; }
}

/// <summary>
/// Represents the result of a lookup operation with a specific result type that includes an optional unit result.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ILookupUnitResult<T> : ILookupResult<T>, ILookupUnitResult
{

}

/// <summary>
/// Represents the result of a lookup operation that includes an optional unit result.
/// </summary>
/// <typeparam name="T">The type of the result.</typeparam>
public record LookupUnitResult<T> : LookupResult<T>, ILookupUnitResult<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LookupUnitResult{T}"/> class with the specified provider type and unit result.
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="result"></param>
    public LookupUnitResult(Type provider, IUnitResult<T> result) : base(provider, result.GetResultOrDefault())
    {
        UnitResult = result;
    }

    /// <summary>
    /// Gets the unit result associated with the lookup operation. This property may be null if a unit result wasn't used by the Lookup.
    /// </summary>
    public IUnitResult? UnitResult { get; }
}