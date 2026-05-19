using IDFCR.Abstractions.Mapper;
using System.Diagnostics.CodeAnalysis;

namespace IDFCR.Abstractions.Results.Extensions;

/// <summary>
/// Extension helpers for unit results.
/// </summary>
public static class UnitResultExtensions
{
    /// <summary>
    /// Gets a value from the result metadata by key and attempts to parse it as an integer. Returns null if the key is not found or if the value cannot be parsed as an integer.
    /// </summary>
    /// <param name="result">The unit result containing the metadata.</param>
    /// <param name="key">The key of the metadata value to retrieve.</param>
    /// <returns>The integer value if found and successfully parsed; otherwise, null.</returns>
    public static int? GetValue(IUnitResult result, string key)
    {
        int? value = null;
        if (result.Meta.TryGetValue(key, out var val)
            && int.TryParse(val?.ToString(), out int _value))
        {
            value = _value;
        }

        return value;
    }

    /// <summary>
    /// Gets the total rows value from the result metadata. Returns null if the key is not found or if the value cannot be parsed as an integer.
    /// </summary>
    /// <param name="result">The unit result containing the metadata.</param>
    /// <returns>The total rows value if found and successfully parsed; otherwise, null.</returns>
    public static int? GetTotalPages(IUnitResult result)
    {
        return GetValue(result, Metadata.Meta.Paging.TotalPages);
    }

    /// <summary>
    /// Gets the page index value from the result metadata. Returns null if the key is not found or if the value cannot be parsed as an integer.
    /// </summary>
    /// <param name="result">The unit result containing the metadata.</param>
    /// <returns>The page index value if found and successfully parsed; otherwise, null.</returns>
    public static int? GetPageIndex(IUnitResult result)
    {
        return GetValue(result, Metadata.Meta.Paging.PageIndex);
    }

    /// <summary>
    /// Gets the page size value from the result metadata. Returns null if the key is not found or if the value cannot be parsed as an integer.
    /// </summary>
    /// <param name="result">The unit result containing the metadata.</param>
    /// <returns>The page size value if found and successfully parsed; otherwise, null.</returns>
    public static int? GetPageSize(IUnitResult result)
    {
        return GetValue(result, Metadata.Meta.Paging.PageSize);
    }

    private static void CloneMeta<TDestination>(IReadOnlyDictionary<string, object?> data, IUnitResult<TDestination> target)
    {
        foreach (var (key, value) in data)
        {
            target.AddMeta(key, value);
        }
    }

    
    /// <summary>
    /// Returns the result value when available, otherwise a supplied default value.
    /// <para>Use this when you don't need to inspect the unit result itself.</para>
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="value">The unit result containing the value.</param>
    /// <param name="default">The default value to return if the result is not available.</param>
    /// <returns>The result value if available; otherwise, the default value.</returns>
    public static T? GetResultOrDefault<T>(this IUnitResult<T> value, T? @default = default)
    {
        return GetResultOrDefault(value, out _, @default);
    }

    /// <summary>
    /// Return the result value when available, otherwise a supplied default value. Also outputs the unit result for further inspection if needed.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="value">The unit result containing the value.</param>
    /// <param name="result">The unit result for further inspection.</param>
    /// <param name="default">The default value to return if the result is not available.</param>
    /// <returns>The result value if available; otherwise, the default value.</returns>
    public static T? GetResultOrDefault<T>(this IUnitResult<T> value, out IUnitResult result, T? @default = default)
    {
        result = value;
        if (value.HasValue)
        {
            return value.Result;
        }

        return @default;
    }

    /// <summary>
    /// Converts a unit result to a paged result using the supplied paged query. The total rows is set to the count of the result collection if available, otherwise 0.
    /// </summary>
    /// <typeparam name="T">The type of the result items.</typeparam>
    /// <param name="unitResult">The unit result collection to convert.</param>
    /// <param name="pagedQuery">The paged query information.</param>
    /// <returns>A paged result containing the items from the unit result collection.</returns>
    public static IUnitPagedResult<T> AsPaged<T>(this IUnitResultCollection<T> unitResult, IPagedQuery pagedQuery)
    {
        var pagedResult = new UnitPagedResult<T>(unitResult.Result, unitResult.Result?.Count() ?? 0, pagedQuery, unitResult.Action, unitResult.IsSuccess, unitResult.Exception);
        CloneMeta(unitResult.Meta, pagedResult);
        return pagedResult;
    }

    /// <summary>
    /// Converts a collection result using the supplied converter.
    /// </summary>
    public static IUnitResultCollection<TDestination> Convert<T, TDestination>(this IUnitResultCollection<T> unitResultCollection, Func<T, TDestination> converter)
    {
        if (!unitResultCollection.HasValue)
        {
            var res = new DefaultUnitResult(unitResultCollection.Exception, unitResultCollection.Action, unitResultCollection.IsSuccess).AsCollection<TDestination>(default);
            CloneMeta(unitResultCollection.Meta, res);
            return res;
        }

        var mapped = unitResultCollection.Result.Select(x => converter(x));
        var result = new UnitResultCollection<TDestination>(mapped, unitResultCollection.Action, unitResultCollection.IsSuccess, unitResultCollection.Exception);
        CloneMeta(unitResultCollection.Meta, result);
        return result;
    }

    /// <summary>
    /// Converts a unit result using the supplied converter.
    /// </summary>
    public static IUnitResult<TDestination> Convert<T, TDestination>(this IUnitResult<T> unitResult, Func<T, TDestination> converter)
    {
        if (!unitResult.HasValue)
        {
            var res = new DefaultUnitResult(unitResult.Exception, unitResult.Action, unitResult.IsSuccess).As<TDestination>(default);
            CloneMeta(unitResult.Meta, res);
            return res;
        }

        var result = new DefaultUnitResult<TDestination>(converter(unitResult.Result), unitResult.Action, unitResult.IsSuccess, unitResult.Exception);
        CloneMeta(unitResult.Meta, result);
        return result;
    }

    /// <summary>
    /// Converts a paged result using the supplied converter.
    /// </summary>
    public static IUnitPagedResult<TDestination> Convert<T, TDestination>(this IUnitPagedResult<T> unitResult, Func<T, TDestination> converter)
    {
        var convertedResults = unitResult.Result?.Select(converter).ToList() ?? [];
        var results = new UnitPagedResult<TDestination>(convertedResults, unitResult.TotalRows, unitResult.PagedQuery, unitResult.Action, unitResult.IsSuccess, unitResult.Exception);
        CloneMeta(unitResult.Meta, results);
        return results;
    }

    /// <summary>
    /// Converts a unit result by mapping between two mapper implementations.
    /// </summary>
    public static IUnitResult<TDestination> Convert<TAbstraction, T, TDestination>(this IUnitResult<T> unitResult)
    where T : IMapper<TAbstraction>, TAbstraction, new()
    where TDestination : IMapper<TAbstraction>, TAbstraction, new()
    {
        return unitResult.Convert(x =>
        {
            var result = new TDestination();
            result.Map(x);
            return result;
        });
    }

    /// <summary>
    /// Checks if the unit result is a chained result and outputs the chained result if it is. Returns true if the result is a chained result, otherwise false.
    /// </summary>
    /// <param name="result">The unit result to check.</param>
    /// <param name="chainedResult">The chained result if the unit result is a chained result, otherwise null.</param>
    /// <returns>True if the unit result is a chained result, otherwise false.</returns>
    public static bool IsChainedResult(this IUnitResult result,
        [NotNullWhen(true)] out IChainedUnitResult? chainedResult)
    {
        chainedResult = default;
        if (result is IChainedUnitResult chained)
        {
            chainedResult = chained;
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// Checks if the unit result is a chained result and outputs the chained result of the specified type if it is. Returns the chained result of the specified type if the unit result is a chained result, otherwise null.
    /// </summary>
    /// <typeparam name="T">The type of the chained result.</typeparam>
    /// <param name="result">The unit result to check.</param>
    /// <returns>The chained result of the specified type if the unit result is a chained result, otherwise null.</returns>
    public static IUnitResult<T>? ChainedResultOf<T>(this IUnitResult result)
    {
        if(!result.IsChainedResult(out var chainedResult))
        {
            return null;
        }

        return chainedResult.Of<T>();
    }

    /// <summary>
    /// Determines whether any or all of the results in a chained unit result satisfy a specified condition defined by the provided expression. This method allows you to evaluate the results in a chained unit result against a given condition and determine if any or all of the results meet that condition based on the specified applicability (either any or all). It returns true if the condition is satisfied according to the specified applicability, otherwise false.
    /// </summary>
    /// <param name="result">The unit result to check.</param>
    /// <param name="expression">The condition to evaluate.</param>
    /// <returns>True if the condition is satisfied according to the specified applicability, otherwise false.</returns>
    public static bool ChainedResultHas(this IUnitResult result, Func<IUnitResult, bool> expression)
    {
#pragma warning disable CS0618
        return ChainedResultHas(result, expression, ApplicableTo.None);
#pragma warning restore
    }

    /// <summary>
    /// Determines whether any or all of the results in a chained unit result satisfy a specified condition defined by the provided expression, based on the specified applicability.
    /// </summary>
    /// <param name="result">The unit result to check.</param>
    /// <param name="expression">The condition to evaluate.</param>
    /// <param name="applicableTo">Specifies whether the condition should be applied to any or all results.</param>
    /// <returns>True if the condition is satisfied according to the specified applicability, otherwise false.</returns>
    public static bool ChainedResultHas(this IUnitResult result, Func<IUnitResult, bool> expression, ApplicableTo applicableTo)
    {
        return result.ChainedResultHas(expression, applicableTo, out _);
    }

    /// <summary>
    /// Determines whether any or all of the results in a chained unit result satisfy a specified condition defined by the provided expression. Also outputs the results that satisfy the condition.
    /// </summary>
    /// <param name="result">The unit result to check.</param>
    /// <param name="expression">The condition to evaluate.</param>
    /// <param name="applicableFilters">Outputs the results that satisfy the condition.</param>
    /// <returns>True if the condition is satisfied according to the specified applicability, otherwise false.</returns>
    public static bool ChainedResultHas(this IUnitResult result, Func<IUnitResult, bool> expression, out IEnumerable<IUnitResult> applicableFilters)
    {
#pragma warning disable CS0618
        return result.ChainedResultHas(expression, ApplicableTo.None, out applicableFilters);
#pragma warning restore
    }

    /// <summary>
    /// Determines whether any or all of the results in a chained unit result satisfy a specified condition defined by the provided expression, based on the specified applicability. Also outputs the results that satisfy the condition.
    /// </summary>
    /// <param name="result">The unit result to check.</param>
    /// <param name="expression">The condition to evaluate.</param>
    /// <param name="applicableTo">Specifies whether the condition should be applied to any or all results.</param>
    /// <param name="applicableFilters">Outputs the results that satisfy the condition.</param>
    /// <returns>True if the condition is satisfied according to the specified applicability, otherwise false.</returns>
    public static bool ChainedResultHas(this IUnitResult result, Func<IUnitResult, bool> expression, ApplicableTo applicableTo, out IEnumerable<IUnitResult> applicableFilters)
    {
        applicableFilters = [];
        if (!result.IsChainedResult(out var chainedResult))
        {
            return false;
        }

        var results = chainedResult.Enumerate();

        applicableFilters = results.Where(expression);

        return applicableTo switch
        {
            ApplicableTo.All => results.All(expression),
            _ => results.Any(expression),
        };
    }

    /// <summary>
    /// Gets the type of the result value from a unit result. If the unit result is a typed unit result, it returns the type of the result value. If the unit result is not a typed unit result, it attempts to find an implemented generic interface of type <see cref="IUnitResult{T}"/> and returns the type argument T. If no such interface is found, it returns null.
    /// </summary>
    /// <param name="result">The unit result to get the type from.</param>
    /// <returns>The type of the result value, or null if it cannot be determined.</returns>
    public static Type? GetResultType(this IUnitResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var concreteType = result.GetType();

        // Fall back to scanning implemented interfaces if the concrete type wrapped it oddly
        if (!concreteType.IsGenericType)
        {
            var genericInterface = concreteType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IUnitResult<>));

            return genericInterface?.GenericTypeArguments.FirstOrDefault();
        }

        return concreteType.GenericTypeArguments.FirstOrDefault();
    }
}
