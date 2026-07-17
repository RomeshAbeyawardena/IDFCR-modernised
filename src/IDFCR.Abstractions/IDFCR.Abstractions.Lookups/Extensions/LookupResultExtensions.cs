using IDFCR.Abstractions.Metadata.Lookups;

namespace IDFCR.Abstractions.Lookups.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="ILookupResults{TResult}"/> interface.
/// </summary>
public static class LookupResultExtensions
{
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
        var lookupProviderResult = lookup.Results.FirstOrDefault(x => x.Provider == provider);
        
        if (lookupProviderResult is null)
        {
            return null;
        }

        return lookupProviderResult.Result;
    }
}
