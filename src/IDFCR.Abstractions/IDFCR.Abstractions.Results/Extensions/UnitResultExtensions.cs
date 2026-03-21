using IDFCR.Abstractions.Mapper;

namespace IDFCR.Abstractions.Results.Extensions;

public static class UnitResultExtensions
{
    private static void CloneMeta<TDestination>(IReadOnlyDictionary<string, object?> data, IUnitResult<TDestination> target)
    {
        foreach (var (key, value) in data)
        {
            target.AddMeta(key, value);
        }
    }

    public static T? GetResultOrDefault<T>(this IUnitResult<T> value, T? @default = default)
    {
        if (value.HasValue)
        {
            return value.Result;
        }

        return @default;
    }

    public static IUnitResultCollection<TDestination> Convert<T, TDestination>(this IUnitResultCollection<T> unitResultCollection, Func<T, TDestination> converter)
    {
        if (unitResultCollection.Result is null)
        {
            var res = new DefaultUnitResult(unitResultCollection.Exception, unitResultCollection.Action, unitResultCollection.IsSuccess).AsCollection<TDestination>(default);
            CloneMeta(unitResultCollection, res);
            return res;
        }

        var mapped = unitResultCollection.Result.Select(x => converter(x));
        var result = new UnitResultCollection<TDestination>(mapped, unitResultCollection.Action, unitResultCollection.IsSuccess, unitResultCollection.Exception);
        CloneMeta(unitResultCollection, result);
        return result;
    }

    public static IUnitResult<TDestination> Convert<T, TDestination>(this IUnitResult<T> unitResult, Func<T, TDestination> converter)
    {
        if (unitResult.Result is null)
        {
            var res = new DefaultUnitResult(unitResult.Exception, unitResult.Action, unitResult.IsSuccess).As<TDestination>(default);
            CloneMeta(unitResult, res);
            return res;
        }

        var result = new DefaultUnitResult<TDestination>(converter(unitResult.Result), unitResult.Action, unitResult.IsSuccess, unitResult.Exception);
        CloneMeta(unitResult, result);
        return result;
    }

    public static IUnitPagedResult<TDestination> Convert<T, TDestination>(this IUnitPagedResult<T> unitResult, Func<T, TDestination> converter)
    {
        var convertedResults = unitResult.Result?.Select(converter).ToList() ?? [];
        var results = new UnitPagedResult<TDestination>(convertedResults, unitResult.TotalRows, unitResult.PagedQuery, unitResult.Action, unitResult.IsSuccess, unitResult.Exception);
        CloneMeta(unitResult, results);
        return results;
    }

    public static IUnitResult<TDestination> Convert<TAbstraction, T, TDestination>(this IUnitResult<T> unitResult)
    where T : IMapper<TAbstraction>, TAbstraction, new()
    where TDestination : IMapper<TAbstraction>, TAbstraction, new()
    {
        return unitResult.Convert(x => {
            var result = new TDestination();
            result.Map(x);
            return result;
        });
    }
}
