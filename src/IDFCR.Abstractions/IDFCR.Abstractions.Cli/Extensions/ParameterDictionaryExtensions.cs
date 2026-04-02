using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Results.Extensions;

namespace IDFCR.Abstractions.Cli.Extensions;

public sealed class KeyNotFoundException(string messsage, Exception? innerException) : Exception(messsage, innerException)
{
    public KeyNotFoundException(string? key = null) : this(string.IsNullOrWhiteSpace(key) 
        ? "Key not found" : $"Key '{key}' not found", null) { }
}

public static class ParameterDictionaryExtensions
{
    public static IUnitResult<TValue> TryGetValueFromKeys<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, params TKey[] keys)
    {
        TKey? currentKey = default!;
        foreach (var key in keys)
        {
            currentKey = key;
            if (dictionary.TryGetValue(key, out TValue? value))
            {
                return UnitResult.FromResult(value);
            }
        }

        return UnitResult.Failed<TValue>(new KeyNotFoundException(currentKey?.ToString()));
    }

    public static bool IsDependencyError<T>(this IUnitResult<T> result, out Exception? innerException)
    {
        innerException = result.Exception;
        return !result.IsSuccess && result.Exception is not KeyNotFoundException;
    }

    public static async Task<IUnitResult<T>> LookupAsync<T>(this IDictionary<string, Parameter> parameters,
        Func<string, CancellationToken, Task<IUnitResult<T>>> lookupAction,
        CancellationToken cancellationToken, 
        params string[] keys)
    {
        var lookupParameterResult = parameters
            .TryGetValueFromKeys(keys);

        var lookupParameter = lookupParameterResult
            .GetResultOrDefault();

        if (lookupParameter is not null
            && !string.IsNullOrWhiteSpace(lookupParameter.Value))
        {
            return await lookupAction(lookupParameter.Value, cancellationToken);
        }

        return UnitResult.Failed<T>(lookupParameterResult.Exception ?? new InvalidOperationException("Unknown state"));
    }

    public static string? TryGetRawValue(this IDictionary<string, Parameter> parameters, params string[] keys)
    {
        foreach (var key in keys)
        {
            if(parameters.TryGetValue(key, out var parameter)
                && !string.IsNullOrWhiteSpace(parameter.Value))
            {
                return parameter.Value;
            }
        }

        return null;
    }

    public static int? GetIntOrDefault(this IDictionary<string, Parameter> parameters, params string[] keys)
    {
        var rawValue  = TryGetRawValue(parameters, keys);
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return null;
        }

        if (int.TryParse(rawValue, out var _result))
        {
            return _result;
        }

        return null;
    }

    public static T ApplyPaging<T>(this IDictionary<string, Parameter> parameters,
        T model)
        where T : PagedQuery
    {
        int pageIndex = 0,
          pageSize = 20;
        
        pageIndex = parameters.GetIntOrDefault("page-index", "i").GetValueOrDefault(pageIndex);
        pageSize = parameters.GetIntOrDefault("page-size", "s").GetValueOrDefault(pageSize);

        model.PageIndex = pageIndex;
        model.PageSize = pageSize;
        return model;
    }
}
