namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents the result of a lookup operation, including the provider type and the result object.
/// </summary>
public interface ILookupResult
{
    /// <summary>
    /// Gets the type of the provider that performed the lookup operation.
    /// </summary>
    Type Provider { get; }
    /// <summary>
    /// Gets the result of the lookup operation, which may be null if no result was found.
    /// </summary>
    object? Result { get; }
}

/// <summary>
/// Represents the result of a lookup operation with a specific result type, including the provider type and the result object.
/// </summary>
/// <typeparam name="T">The type of the result object.</typeparam>
public interface ILookupResult<T> : ILookupResult
{
    /// <summary>
    /// Gets the result of the lookup operation, which may be null if no result was found.
    /// </summary>
    new T? Result { get; }
}