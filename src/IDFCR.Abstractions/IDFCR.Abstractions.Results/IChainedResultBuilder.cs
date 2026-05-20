namespace IDFCR.Abstractions.Results;

/// <summary>
/// Represents a builder for creating chained unit results, allowing for the construction of complex result chains by adding individual unit results and then building a final chained unit result that combines the information from all the added results. This interface provides methods to add individual unit results to the builder and to build a final chained unit result based on the added results, facilitating the creation of cohesive result chains that preserve the information and status of each individual result in the chain.
/// </summary>
public interface IChainedResultBuilder
{
    /// <summary>
    /// Adds an individual unit result to the builder, allowing it to be included in the final chained unit result that will be built. This method takes an instance of IUnitResult as a parameter and adds it to the internal collection of results that will be used to construct the final chained unit result when the Build method is called. By adding multiple unit results using this method, you can create a chain of results that captures the information and status of each individual result in the chain, which can then be combined into a cohesive final result when the Build method is invoked.
    /// </summary>
    /// <param name="result">The unit result to add to the builder.</param>
    void Add(IUnitResult result);
    /// <summary>
    /// Builds a final chained unit result based on the individual unit results that have been added to the builder. This method takes an instance of IUnitResult as a parameter, which represents the initial result that will be included in the chain. The method then combines this initial result with all the previously added results to create a new chained unit result that contains the information from all the results in the chain. The resulting chained unit result preserves the information and status of each individual result in the chain, allowing for a cohesive representation of the combined results when this method is called. There is also an overload of this method that accepts an <see cref="IUnitResult{T}"/> parameter, allowing for the creation of a typed chained unit result that includes a value of type T in addition to the information from the individual results in the chain.
    /// </summary>
    /// <param name="result">The initial unit result to include in the chain.</param>
    /// <returns>A chained unit result containing the information from all the results in the chain.</returns>
    IChainedUnitResult Build(IUnitResult result);
    /// <summary>
    /// Builds a final chained unit result based on the individual unit results that have been added to the builder, with support for a typed initial result. This method takes an instance of <see cref="IUnitResult{T}"/> as a parameter, which represents the initial result that will be included in the chain and contains a value of type T. The method then combines this initial typed result with all the previously added results to create a new chained unit result that contains the information from all the results in the chain, including the typed value from the initial result. The resulting chained unit result preserves the information and status of each individual result in the chain, allowing for a cohesive representation of the combined results when this method is called, while also providing access to the typed value from the initial result in the chain.
    /// </summary>
    /// <typeparam name="T">The type of the result value in the initial unit result.</typeparam>
    /// <param name="result">The initial typed unit result to include in the chain.</param>
    /// <returns>A typed chained unit result containing the information from all the results in the chain, including the typed value from the initial result.</returns>
    IChainedUnitResult<T> Build<T>(IUnitResult<T> result);
}