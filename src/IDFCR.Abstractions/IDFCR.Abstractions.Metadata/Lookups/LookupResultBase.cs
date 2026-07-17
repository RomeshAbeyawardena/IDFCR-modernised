namespace IDFCR.Abstractions.Metadata.Lookups;

/// <summary>
/// Represents the base class for lookup results, including the provider type and the result object.
/// </summary>
public abstract record LookupResultBase : ILookupResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LookupResultBase"/> class with the specified provider type and result object.
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="result"></param>
    protected LookupResultBase(Type provider, object? result)
    {
        Provider = provider;
        BaseResult = result;
    }

    /// <summary>
    /// Gets the type of the provider that performed the lookup operation.
    /// </summary>
    public Type Provider { get; }
    /// <summary>
    /// Gets the result of the lookup operation, which may be null if no result was found.
    /// </summary>
    protected object? BaseResult { get; }

    object? ILookupResult.Result => BaseResult;
}

/// <summary>
/// Represents the base class for lookup results with a specific result type, including the provider type and the result object.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract record LookupResultBase<T> : LookupResultBase, ILookupResult<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LookupResultBase{T}"/> class with the specified provider type and result object.
    /// </summary>
    /// <param name="provider">The type of the provider that performed the lookup operation.</param>
    /// <param name="result">The result of the lookup operation, which may be null if no result was found.</param>
    protected LookupResultBase(Type provider, T? result) : base(provider, result)
    {
        Result = result;
    }

    /// <summary>
    /// Gets the result of the lookup operation, which may be null if no result was found.
    /// </summary>
    public virtual T? Result { get; }
}