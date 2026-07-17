using IDFCR.Abstractions.Metadata.Lookups;

namespace IDFCR.Abstractions.Lookups.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="ILookupResults{TResult}"/> interface.
/// </summary>
public static class LookupResultExtensions
{
    /// <summary>
    /// Gets the first result from the lookup results or <c>null</c> if no results are available.
    /// If multiple providers were invoked their results will be omitted.
    /// </summary>
    /// <typeparam name="TResult">The type of the lookup result.</typeparam>
    /// <param name="lookup">The lookup results.</param>
    /// <returns>The first result, or <c>null</c> if no results are available.</returns>
    public static TResult? GetResult<TResult>(this ILookupResults<TResult> lookup)
        where TResult : class
    {
        ArgumentNullException.ThrowIfNull(lookup, nameof(lookup));
        var providerResult = lookup.Results.FirstOrDefault();

        if (providerResult is null)
        {
            return null;
        }

        return providerResult.Result;
    }

    /// <summary>
    /// Gets the result of a specific provider from the lookup results.
    /// </summary>
    /// <typeparam name="TResult">The type of the lookup result.</typeparam>
    /// <param name="lookup">The lookup results.</param>
    /// <param name="provider">The provider type.</param>
    /// <returns>The result of the specified provider, or <c>null</c> if not found.</returns>
    public static TResult? GetResult<TResult>(this ILookupResults<TResult> lookup, Type provider)
        where TResult : class
    {
        ArgumentNullException.ThrowIfNull(lookup, nameof(lookup));

        bool providerPredicate(ILookupResult<TResult> x) => x.Provider == provider;

        if (!lookup.Results.Any(providerPredicate))
        {
            return null;
        }

        return lookup.Results.First(providerPredicate).Result;
    }

    /// <summary>
    /// Gets the result of a specific provider from the lookup results by provider name.
    /// </summary>
    /// <typeparam name="TResult">The type of the lookup result.</typeparam>
    /// <param name="lookup">The lookup results.</param>
    /// <param name="providerName">The name of the provider.</param>
    /// <returns>The result of the specified provider, or <c>null</c> if not found.</returns>
    public static TResult? GetResult<TResult>(this ILookupResults<TResult> lookup, string providerName)
        where TResult : class
    {
        ArgumentNullException.ThrowIfNull(lookup, nameof(lookup));
        ArgumentException.ThrowIfNullOrWhiteSpace(providerName, nameof(providerName));

        bool providerPredicate(ILookupResult<TResult> x) => x.Provider.Name == providerName;

        if (!lookup.Results.Any(providerPredicate))
        {
            return null;
        }

        return lookup.Results.First(providerPredicate).Result;
    }
}
