namespace IDFCR.Abstractions.Metadata.Lookups;

/// <summary>
/// Represents a builder for constructing lookup results, allowing the addition and removal of individual lookup results before building the final collection of results.
/// </summary>
/// <typeparam name="T">The type of the elements in the result collection.</typeparam>
public class LookupResultsBuilder<T>
    : ILookupResultsBuilder<T>
{
    private readonly List<ILookupResult<T>> results = [];

    /// <summary>
    /// Adds a lookup result to the builder, associating it with the specified provider type and result item.
    /// </summary>
    /// <param name="provider">The type of the provider associated with the lookup result.</param>
    /// <param name="item">The result item to be added.</param>
    /// <returns>The current instance of the builder, allowing for method chaining.</returns>
    public ILookupResultsBuilder<T> Add(Type provider, T? item)
    {
        return Add(new LookupResult<T>(provider, item));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public ILookupResultsBuilder<T> Add(ILookupResult<T> result)
    {
        results.Add(result);
        return this;
    }

    /// <summary>
    /// Builds the final collection of lookup results, returning an instance of <see cref="ILookupResults{T}"/> that contains all the added results.
    /// </summary>
    /// <returns>An instance of <see cref="ILookupResults{T}"/> containing all the added results.</returns>
    public ILookupResults<T> Build()
    {
        return new LookupResults<T>(results);
    }

    /// <summary>
    /// Removes a lookup result from the builder, based on the specified provider type and result item.
    /// </summary>
    /// <param name="provider">The type of the provider associated with the lookup result to be removed.</param>
    /// <param name="item">The result item to be removed.</param>
    /// <returns>The current instance of the builder, allowing for method chaining.</returns>
    public ILookupResultsBuilder<T> Remove(Type provider, T? item)
    {
        var resultToRemove = results.FirstOrDefault(r => r.Provider == provider && EqualityComparer<T>.Default.Equals(r.Result, item));
        if (resultToRemove != null)
        {
            results.Remove(resultToRemove);
        }

        return this;
    }
}
