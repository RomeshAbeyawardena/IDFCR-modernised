namespace IDFCR.Abstractions.Results.Extensions;

/// <summary>
/// Represents a utility class for chaining unit results together, providing methods to create new chained unit results that combine the information from multiple unit results. This class allows for the creation of complex result chains while preserving the information and status of each individual result in the chain.
/// </summary>
public static class ChainedUnitResultExtensions
{
    /// <summary>
    /// Chains multiple unit results together, creating a new chained unit result that contains the information from all the results in the chain. The first result becomes the last result in the chain, while the last result's information is preserved and accessible through the Last property of the resulting chained unit result. This method allows for chaining an arbitrary number of unit results together, providing a convenient way to combine multiple results into a single cohesive result chain.
    /// </summary>
    /// <param name="result">The first unit result in the chain.</param>
    /// <param name="results">The subsequent unit results to chain.</param>
    /// <param name="setAsFailWhenAnyUnitsFail">
    /// When true, the built chained result is marked as failed if the initial result
    /// or any added result has failed. When false, added results are preserved in the
    /// chain but do not affect the success state of the built result.
    ///</param>
    /// <returns>A chained unit result containing the information from all the results in the chain.</returns>
    public static IChainedUnitResult<T> Chain<T>(this IUnitResult<T> result, IEnumerable<IUnitResult> results, bool setAsFailWhenAnyUnitsFail = false)
    {
        using IEnumerator<IUnitResult> enumerator = results.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return result as IChainedUnitResult<T> 
                ?? new DefaultChainedUnitResult<T>(result, UnitResult.Create(true), setAsFailWhenAnyUnitsFail);
        }

        IChainedUnitResult<T> currentChain = new DefaultChainedUnitResult<T>(result, enumerator.Current);

        while (enumerator.MoveNext())
        {
            if (enumerator.Current is IChainedUnitResult chainedUnitResult)
            {
                currentChain = currentChain.Chain(chainedUnitResult.Enumerate());
                continue;
            }

            currentChain = new DefaultChainedUnitResult<T>(currentChain, enumerator.Current, setAsFailWhenAnyUnitsFail);
        }

        return currentChain;
    }

    /// <summary>
    /// Chains multiple unit results together, creating a new chained unit result that contains the information from all the results in the chain. The first result becomes the last result in the chain, while the last result's information is preserved and accessible through the Last property of the resulting chained unit result. This method allows for chaining an arbitrary number of unit results together, providing a convenient way to combine multiple results into a single cohesive result chain.
    /// </summary>
    /// <param name="result">The first unit result in the chain.</param>
    /// <param name="results">The subsequent unit results to chain.</param>
    /// <param name="setAsFailWhenAnyUnitsFail">
    /// When true, the built chained result is marked as failed if the initial result
    /// or any added result has failed. When false, added results are preserved in the
    /// chain but do not affect the success state of the built result.
    ///</param>
    /// <returns>A chained unit result containing the information from all the results in the chain.</returns>
    public static IChainedUnitResult Chain(this IUnitResult result, IEnumerable<IUnitResult> results, bool setAsFailWhenAnyUnitsFail = false)
    {
        using IEnumerator<IUnitResult> enumerator = results.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return result as IChainedUnitResult 
                ?? new DefaultChainedUnitResult(result, UnitResult.Create(true), setAsFailWhenAnyUnitsFail);
        }

        IChainedUnitResult currentChain = new DefaultChainedUnitResult(result, enumerator.Current);

        while (enumerator.MoveNext())
        {
            currentChain = new DefaultChainedUnitResult(currentChain, enumerator.Current, setAsFailWhenAnyUnitsFail);
        }

        return currentChain;
    }
    /// <summary>
    /// Chains two unit results together, creating a new chained unit result that contains the information from both results. The second result becomes the last result in the chain, while the first result's information is preserved and accessible through the Last property of the resulting chained unit result.
    /// </summary>
    /// <param name="lastResult">The first unit result in the chain. This result's information will be preserved and accessible through the Last property of the resulting chained unit result.</param>
    /// <param name="currentResult">The second unit result in the chain. This result becomes the last result in the chain.</param>
    /// <param name="setAsFailWhenAnyUnitsFail">
    /// When true, the built chained result is marked as failed if the initial result
    /// or any added result has failed. When false, added results are preserved in the
    /// chain but do not affect the success state of the built result.
    ///</param>
    /// <returns></returns>
    public static IChainedUnitResult Chain(this IUnitResult currentResult, IUnitResult lastResult, bool setAsFailWhenAnyUnitsFail = false)
    { 
        return new DefaultChainedUnitResult(currentResult, lastResult, setAsFailWhenAnyUnitsFail);
    }

    /// <summary>
    /// Chains two unit results together, creating a new chained unit result that contains the information from both results. The second result becomes the last result in the chain, while the first result's information is preserved and accessible through the Last property of the resulting chained unit result.
    /// </summary>
    /// <typeparam name="T">The type of the result value in the first unit result.</typeparam>
    /// <param name="lastResult">The last result </param>
    /// <param name="currentResult">The second unit result in the chain. This result becomes the last result in the chain.</param>
    /// <param name="setAsFailWhenAnyUnitsFail">
    /// When true, the built chained result is marked as failed if the initial result
    /// or any added result has failed. When false, added results are preserved in the
    /// chain but do not affect the success state of the built result.
    ///</param>
    public static IChainedUnitResult<T> Chain<T>(this IUnitResult<T> currentResult, IUnitResult lastResult, bool setAsFailWhenAnyUnitsFail = false)
    {
        return new DefaultChainedUnitResult<T>(currentResult, lastResult, setAsFailWhenAnyUnitsFail);
    }

    /// <summary>
    /// Retrieves the first unit result of a specified type from a chained unit result, optionally filtered by a provided predicate. This method allows you to search through the chain of unit results and find the first result that matches the specified type and satisfies the given condition defined by the predicate. If no matching result is found, it returns null.
    /// </summary>
    /// <typeparam name="T">The type of the result value to search for in the chained unit result.</typeparam>
    /// <param name="chainedUnitResult">The chained unit result to search through.</param>
    /// <param name="predicate">An optional predicate to filter the results.</param>
    /// <returns>The first unit result of the specified type that matches the predicate, or null if no such result is found.</returns>
    public static IUnitResult<T> Of<T>(this IChainedUnitResult chainedUnitResult, Func<IUnitResult<T>, bool> predicate)
    {
        var result = chainedUnitResult.Enumerate()
            .OfType<IUnitResult<T>>()
            .FirstOrDefault(predicate);

        if (result is null)
        {
            return UnitResult.NotFound<T>(predicate, new NullReferenceException("Chained result not found"), failureOrigin: FailureOrigin.Internal);
        }

        return result;
    }

    /// <summary>
    /// Retrieves the first unit result of a specified type from a chained unit result that has a parent with a specific key. This method allows you to search through the chain of unit results and find the first result that matches the specified type and is associated with a parent that has the given key. If no matching result is found, it returns null.
    /// </summary>
    /// <typeparam name="T">The type of the result value to search for in the chained unit result.</typeparam>
    /// <param name="chainedUnitResult">The chained unit result to search through.</param>
    /// <param name="key">The key of the parent unit result to match.</param>
    /// <returns>The first unit result of the specified type that has a parent with the given key, or null if no such result is found.</returns>
    public static IUnitResult<T> Of<T>(this IChainedUnitResult chainedUnitResult, string key)
    {
        var result =  chainedUnitResult.EnumerateWithParents()
            .Where(x => x.Parent?.Key == key)
            .Select(x => x.Result)
            .OfType<IUnitResult<T>>()
            .FirstOrDefault();

        if (result is null)
        {
            return UnitResult.NotFound<T>(key, new NullReferenceException("Chained result not found"), failureOrigin: FailureOrigin.Internal);
        }

        return result;
    }

    /// <summary>
    /// Retrieves the first unit result of a specified type from a chained unit result that has a value, without applying any additional filtering. This method is a convenience overload of the OfType method that automatically filters for results that have a value, allowing you to quickly find the first successful result of the specified type in the chain. If no such result is found, it returns null.
    /// </summary>
    /// <typeparam name="T">The type of the result value to search for in the chained unit result.</typeparam>
    /// <param name="chainedUnitResult">The chained unit result to search through.</param>
    /// <returns>The first unit result of the specified type that has a value, or null if no such result is found.</returns>
    public static IUnitResult<T> Of<T>(this IChainedUnitResult chainedUnitResult)
    {
        return Of<T>(chainedUnitResult, x => x.HasValue);
    }
}