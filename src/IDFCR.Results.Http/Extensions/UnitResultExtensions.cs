using IDFCR.Abstractions.Results;

namespace IDFCR.Results.Http.Extensions;

/// <summary>
/// Defines extension methods for converting IUnitResult and <see cref="IUnitResult{T}"/> into IUnitHttpResult, which can be used to create HTTP responses in an API. These extension methods allow you to easily convert unit results into HTTP results that can be returned from API endpoints, providing a convenient way to handle the conversion between the result of an operation and the corresponding HTTP response.
/// </summary>
public static class UnitResultExtensions
{
    /// <summary>
    /// Creates an IUnitHttpResult from an IUnitResult. This method allows you to easily convert a unit result into an HTTP result that can be returned from an API endpoint.
    /// </summary>
    /// <param name="result">The IUnitResult to convert.</param>
    /// <returns>An IUnitHttpResult that represents the HTTP response for the given unit result.</returns>
    public static IUnitHttpResult AsHttp(this Abstractions.Results.IUnitResult result) => new UnitHttpResult(result);
    /// <summary>
    /// Creates an IUnitHttpResult from an <see cref="IUnitResult{T}"/>. This method allows you to easily convert a unit result with a value into an HTTP result that can be returned from an API endpoint.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the <see cref="IUnitResult{T}"/>.</typeparam>
    /// <param name="result">The <see cref="IUnitResult{T}"/> to convert.</param>
    /// <returns>An IUnitHttpResult that represents the HTTP response for the given unit result with a value.</returns>
    public static IUnitHttpResult AsHttp<T>(this Abstractions.Results.IUnitResult<T> result) => new UnitHttpResult<T>(result);

    /// <summary>
    /// Creates an IUnitHttpResult from an <see cref="IUnitResultCollection{T}"/>. This method allows you to easily convert a unit result that represents a collection of items into an HTTP result that can be returned from an API endpoint. A unit result collection represents the outcome of an operation that may have produced multiple items, and this method will create an HTTP result that reflects the final outcome of that operation, including the collection of items if available.
    /// </summary>
    /// <typeparam name="T">The type of the items contained in the <see cref="IUnitResultCollection{T}"/>.</typeparam>
    /// <param name="result">The <see cref="IUnitResultCollection{T}"/> to convert.</param>
    /// <returns>An IUnitHttpResult that represents the HTTP response for the given unit result collection.</returns>
    public static IUnitHttpResult AsHttp<T>(this IUnitResultCollection<T> result) => new UnitHttpResultCollection<T>(result);

    /// <summary>
    /// Creates an IUnitHttpResult from an <see cref="IChainedUnitResult{T}"/>. This method allows you to easily convert a chained unit result with a value into an HTTP result that can be returned from an API endpoint. A chained unit result represents a sequence of operations that may have been executed, and this method will create an HTTP result that reflects the final outcome of those operations.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the <see cref="IChainedUnitResult{T}"/>.</typeparam>
    /// <param name="result">The <see cref="IChainedUnitResult{T}"/> to convert.</param>
    /// <returns>An IUnitHttpResult that represents the HTTP response for the given chained unit result with a value.</returns>
    public static IUnitHttpResult AsHttp<T>(this IChainedUnitResult<T> result) => new ChainedUnitHttpResult<T>(result);
}
