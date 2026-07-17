namespace IDFCR.Abstractions.Metadata.Lookups;

/// <summary>
/// Represents a builder for constructing lookup results, allowing the addition and removal of results from different providers.
/// </summary>
/// <typeparam name="T">The type of the elements in the result collection.</typeparam>
public interface ILookupResultsBuilder<T>
{
    /// <summary>
    /// Adds a result item from a specific provider to the lookup results.
    /// </summary>
    /// <param name="provider">The type of the provider associated with the result item.</param>
    /// <param name="item">The result item to be added.</param>
    /// <returns>The current instance of the builder, allowing for method chaining.</returns>
    ILookupResultsBuilder<T> Add(Type provider, T? item);
    /// <summary>
    /// Removes a result item from a specific provider from the lookup results.
    /// </summary>
    /// <param name="provider">The type of the provider associated with the result item.</param>
    /// <param name="item">The result item to be removed.</param>
    /// <returns>The current instance of the builder, allowing for method chaining.S</returns>
    ILookupResultsBuilder<T> Remove(Type provider, T? item);

    /// <summary>
    /// Builds the lookup results based on the added and removed items, returning an instance of <see cref="ILookupResults{T}"/>.
    /// </summary>
    /// <returns>An instance of <see cref="ILookupResults{T}"/> containing the constructed lookup results.</returns>
    ILookupResults<T> Build();
}
