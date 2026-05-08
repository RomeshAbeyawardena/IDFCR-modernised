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
        return GetValue(result, "totalPages");
    }

    /// <summary>
    /// Gets the page index value from the result metadata. Returns null if the key is not found or if the value cannot be parsed as an integer.
    /// </summary>
    /// <param name="result">The unit result containing the metadata.</param>
    /// <returns>The page index value if found and successfully parsed; otherwise, null.</returns>
    public static int? GetPageIndex(IUnitResult result)
    {
        return GetValue(result, "pageIndex");
    }

    /// <summary>
    /// Gets the page size value from the result metadata. Returns null if the key is not found or if the value cannot be parsed as an integer.
    /// </summary>
    /// <param name="result">The unit result containing the metadata.</param>
    /// <returns>The page size value if found and successfully parsed; otherwise, null.</returns>
    public static int? GetPageSize(IUnitResult result)
    {
        return GetValue(result, "pageSize");
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
    /// </summary>
    public static T? GetResultOrDefault<T>(this IUnitResult<T> value, T? @default = default)
    {
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
}
