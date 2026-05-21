namespace IDFCR.Abstractions.Results;

/// <summary>
/// Represents a unit result that is part of a chain of results, allowing access to the previous result in the chain.
/// </summary>
public interface IChainedUnitResult : IUnitResult
{
    /// <summary>
    /// Gets the key associated with the current unit result in the chain, allowing identification and differentiation of results within the chain. This property provides a way to label and organize results based on their context or purpose, enabling easier tracking and analysis of the sequence of results in the chain.
    /// </summary>
    string? Key { get; }

    /// <summary>
    /// Sets the key for the current unit result in the chain, allowing identification and differentiation of results within the chain. This method provides a way to label and organize results based on their context or purpose, enabling easier tracking and analysis of the sequence of results in the chain. By assigning a key to each result, it becomes easier to understand the flow of results and identify specific points of interest or concern within the chain.
    /// </summary>
    /// <param name="key">The key to assign to the current unit result.</param>
    /// <returns>The current chained unit result with the updated key.</returns>
    IChainedUnitResult WithKey(string key);

    /// <summary>
    /// Gets the current unit result in the chain, allowing access to the information and status of the current result. This property represents the most recent result in the chain, while the Last property allows access to the previous result's information and status.
    /// </summary>
    IUnitResult Current { get; }
    /// <summary>
    /// Gets the last unit result in the chain, allowing access to the previous result's information and status.
    /// </summary>
    IUnitResult Last { get; }

    /// <summary>
    /// Gets the root unit result in the chain, which is the initial result that started the chain of results. This method allows access to the original result's information and status, providing a way to trace back through the chain of results to the starting point.
    /// </summary>
    /// <returns></returns>
    IUnitResult GetRoot();
    /// <summary>
    /// Gets the first failure unit result in the chain, which is the earliest result that indicates a failure. This method allows access to the information and status of the first failure in the chain, providing insight into where the failure occurred in the sequence of results.
    /// </summary>
    /// <returns></returns>
    IUnitResult GetFirstFailure();
    /// <summary>
    /// Enumerates all unit results in the chain, allowing access to each result's information and status in the order they were added to the chain. This method provides a way to iterate through the entire sequence of results, from the root to the current result, enabling analysis of the progression of results and identification of any patterns or issues that may have occurred along the way.
    /// </summary>
    /// <returns></returns>
    IEnumerable<IUnitResult> Enumerate();
    /// <summary>
    /// Gets the deepest unit result in the chain, which is the most recent result that has been added to the chain. This method allows access to the information and status of the latest result, providing insight into the current state of the chain of results and any recent developments or changes that may have occurred.
    /// </summary>
    /// <returns></returns>
    IUnitResult GetDeepest();

}

/// <summary>
/// Represents a unit result that is part of a chain of results, allowing access to the previous result in the chain and containing a value of type T.
/// </summary>
/// <typeparam name="T">The type of the value contained in the result.</typeparam>
public interface IChainedUnitResult<T> : IUnitResult<T>, IChainedUnitResult
{
    /// <summary>
    /// Sets the key for the current unit result in the chain, allowing identification and differentiation of results within the chain. This method provides a way to label and organize results based on their context or purpose, enabling easier tracking and analysis of the sequence of results in the chain. By assigning a key to each result, it becomes easier to understand the flow of results and identify specific points of interest or concern within the chain.
    /// </summary>
    /// <param name="key">The key to assign to the current unit result.</param>
    /// <returns>The current chained unit result with the updated key.</returns>
    new IChainedUnitResult<T> WithKey(string key);
    /// <summary>
    /// Gets the current unit result in the chain, allowing access to the information and status of the current result. This property represents the most recent result in the chain, while the Last property allows access to the previous result's information and status.
    /// </summary>
    new IUnitResult<T> Current { get; }
}