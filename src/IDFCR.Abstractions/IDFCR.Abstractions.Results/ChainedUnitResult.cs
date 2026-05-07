namespace IDFCR.Abstractions.Results;

/// <summary>
/// Represents a utility class for chaining unit results together, providing methods to create new chained unit results that combine the information from multiple unit results. This class allows for the creation of complex result chains while preserving the information and status of each individual result in the chain.
/// </summary>
public static class ChainedUnitResult
{
    /// <summary>
    /// Chains two unit results together, creating a new chained unit result that contains the information from both results. The second result becomes the last result in the chain, while the first result's information is preserved and accessible through the Last property of the resulting chained unit result.
    /// </summary>
    /// <param name="firstUnitResult">The first unit result in the chain. This result's information will be preserved and accessible through the Last property of the resulting chained unit result.</param>
    /// <param name="secondUnitResult">The second unit result in the chain. This result becomes the last result in the chain.</param>
    /// <param name="setAsFailWhenAnyUnitsFail">Determines whether the resulting chained unit result should be considered a failure if any of the individual unit results in the chain are failures. If set to true, the resulting chained unit result will be marked as a failure if either the first or second unit result is a failure. If set to false, the resulting chained unit result will only be marked as a failure if the first unit result is a failure, regardless of the status of the second unit result.</param>
    /// <returns></returns>
    public static IChainedUnitResult Chain(this IUnitResult firstUnitResult, IUnitResult secondUnitResult, bool setAsFailWhenAnyUnitsFail = true)
    { 
        return new DefaultChainedUnitResult(firstUnitResult, secondUnitResult, setAsFailWhenAnyUnitsFail);
    }

    /// <summary>
    /// Chains two unit results together, creating a new chained unit result that contains the information from both results. The second result becomes the last result in the chain, while the first result's information is preserved and accessible through the Last property of the resulting chained unit result.
    /// </summary>
    /// <typeparam name="T">The type of the result value in the first unit result.</typeparam>
    /// <param name="firstUnitResult">The first unit result in the chain. This result's information will be preserved and accessible through the Last property of the resulting chained unit result.</param>
    /// <param name="secondUnitResult">The second unit result in the chain. This result becomes the last result in the chain.</param>
    /// <param name="setAsFailWhenAnyUnitsFail">Determines whether the resulting chained unit result should be considered a failure if any of the individual unit results in the chain are failures. If set to true, the resulting chained unit result will be marked as a failure if either the first or second unit result is a failure. If set to false, the resulting chained unit result will only be marked as a failure if the first unit result is a failure, regardless of the status of the second unit result.</param>
    /// <returns></returns>
    public static IChainedUnitResult<T> Chain<T>(this IUnitResult<T> firstUnitResult, IUnitResult secondUnitResult, bool setAsFailWhenAnyUnitsFail = true)
    {
        return new DefaultChainedUnitResult<T>(firstUnitResult, secondUnitResult, setAsFailWhenAnyUnitsFail);
    }
}