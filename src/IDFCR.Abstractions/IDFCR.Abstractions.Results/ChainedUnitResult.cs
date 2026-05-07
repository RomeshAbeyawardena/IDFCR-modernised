namespace IDFCR.Abstractions.Results;

/// <summary>
/// Represents a utility class for chaining unit results together, providing methods to create new chained unit results that combine the information from multiple unit results. This class allows for the creation of complex result chains while preserving the information and status of each individual result in the chain.
/// </summary>
public static class ChainedUnitResult
{
    /// <summary>
    /// Chains two unit results together, creating a new chained unit result that contains the information from both results. The second result becomes the last result in the chain, while the first result's information is preserved and accessible through the Last property of the resulting chained unit result.
    /// </summary>
    /// <param name="firstUnitResult"></param>
    /// <param name="secondUnitResult"></param>
    /// <returns></returns>
    public static IChainedUnitResult Chain(this IUnitResult firstUnitResult, IUnitResult secondUnitResult)
    { 
        return new DefaultChainedUnitResult(firstUnitResult, secondUnitResult);
    }

    /// <summary>
    /// Chains two unit results together, creating a new chained unit result that contains the information from both results. The second result becomes the last result in the chain, while the first result's information is preserved and accessible through the Last property of the resulting chained unit result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="firstUnitResult"></param>
    /// <param name="secondUnitResult"></param>
    /// <returns></returns>
    public static IChainedUnitResult<T> Chain<T>(this IUnitResult<T> firstUnitResult, IUnitResult secondUnitResult)
    {
        return new DefaultChainedUnitResult<T>(firstUnitResult, secondUnitResult);
    }
}