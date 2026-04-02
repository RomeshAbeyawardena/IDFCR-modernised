using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Results.Extensions;

namespace IDFCR.Abstractions.Cli.Extensions;

/// <summary>
/// Defines extension methods for <see cref="IDictionary{TKey, TValue}"/>  to facilitate common operations such as trying to retrieve values from multiple keys, checking for dependency errors in results, and applying paging parameters to models. These methods enhance the functionality of dictionaries in the context of command-line interfaces by providing convenient ways to access parameter values and handle results effectively.
/// </summary>
public static class ParameterDictionaryExtensions
{
    /// <summary>
    /// Defines a method to attempt to retrieve a value from a dictionary using multiple keys. It iterates through the provided keys and tries to get the corresponding value from the dictionary. If a value is found for any of the keys, it returns a successful result containing the value. If none of the keys yield a value, it returns a failed result with a KeyNotFoundException indicating the last key that was attempted. This method is useful for scenarios where multiple potential keys may be used to access a value in a dictionary, providing flexibility in key usage.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="dictionary">The dictionary to search for values.</param>
    /// <param name="keys">An array of keys to attempt to retrieve values for.</param>
    /// <returns>A result containing the value if found, or a failure if none of the keys yield a value.</returns>
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

    /// <summary>
    /// Defines a method to check if a result indicates a dependency error. It evaluates whether the result is unsuccessful and if the associated exception is not a KeyNotFoundException. If both conditions are met, it considers the error to be a dependency error and returns true, along with the inner exception. This method is useful for distinguishing between different types of errors in results, allowing for more specific error handling based on the nature of the failure.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="innerException">The inner exception associated with the result, if any.</param>
    /// <returns>True if the result indicates a dependency error; otherwise, false.</returns>
    public static bool IsDependencyError<T>(this IUnitResult<T> result, out Exception? innerException)
    {
        innerException = result.Exception;
        return !result.IsSuccess && result.Exception is not KeyNotFoundException;
    }

    /// <summary>
    /// Defines a method to perform a lookup operation based on values retrieved from a dictionary using multiple keys. It attempts to retrieve a parameter value from the dictionary using the provided keys, and if a valid value is found, it invokes the specified lookup action with the retrieved value and a cancellation token. The method returns the result of the lookup action if successful, or a failed result if no valid parameter value is found or if an error occurs during retrieval. This method is useful for scenarios where a lookup operation needs to be performed based on parameters that may be accessed through multiple potential keys in a dictionary, providing flexibility and error handling in the lookup process.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="parameters">The dictionary of parameters to search.</param>
    /// <param name="lookupAction">The action to perform if a valid parameter value is found.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <param name="keys"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Defines a method to attempt to retrieve a raw string value from a dictionary of parameters using multiple keys. It iterates through the provided keys and tries to get the corresponding parameter value from the dictionary. If a valid parameter value is found for any of the keys, it returns the raw string value. If none of the keys yield a valid parameter value, it returns null. This method is useful for scenarios where multiple potential keys may be used to access a parameter value in a dictionary, providing flexibility in key usage while ensuring that only valid, non-empty values are returned.
    /// </summary>
    /// <param name="parameters">The dictionary of parameters to search.</param>
    /// <param name="keys">The keys to use for the lookup.</param>
    /// <returns>The raw string value if found; otherwise, null.</returns>
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

    /// <summary>
    /// Defines a method to attempt to retrieve an integer value from a dictionary of parameters using multiple keys. It utilizes the TryGetRawValue method to get the raw string value associated with the provided keys, and if a valid raw value is found, it attempts to parse it as an integer. If the parsing is successful, it returns the integer value; otherwise, it returns null. This method is useful for scenarios where multiple potential keys may be used to access an integer parameter value in a dictionary, providing flexibility in key usage while ensuring that only valid integer values are returned.
    /// </summary>
    /// <param name="parameters">The dictionary of parameters to search.</param>
    /// <param name="keys">The keys to use for the lookup.</param>
    /// <returns>The integer value if found and successfully parsed; otherwise, null.</returns>
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

    /// <summary>
    /// Defines a method to apply paging parameters to a model that implements the PagedQuery interface. It retrieves the page index and page size from the dictionary of parameters using the GetIntOrDefault method with specified keys for page index and page size. If valid values are found, it sets the PageIndex and PageSize properties of the model accordingly. This method is useful for scenarios where paging parameters need to be applied to a query model based on user input or command-line parameters, providing a convenient way to configure paging settings for queries in a consistent manner.
    /// </summary>
    /// <typeparam name="T">The type of the model that implements the PagedQuery interface.</typeparam>
    /// <param name="parameters">The dictionary of parameters to search.</param>
    /// <param name="model">The model to apply the paging parameters to.</param>
    /// <returns>The model with the applied paging parameters.</returns>
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
