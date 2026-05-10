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
    public static IUnitHttpResult AsHttp(this IUnitResult result) => new UnitHttpResult(result);
    /// <summary>
    /// Creates an IUnitHttpResult from an <see cref="IUnitResult{T}"/>. This method allows you to easily convert a unit result with a value into an HTTP result that can be returned from an API endpoint.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the <see cref="IUnitResult{T}"/>.</typeparam>
    /// <param name="result">The <see cref="IUnitResult{T}"/> to convert.</param>
    /// <returns>An IUnitHttpResult that represents the HTTP response for the given unit result with a value.</returns>
    public static IUnitHttpResult AsHttp<T>(this IUnitResult<T> result) => new UnitHttpResult<T>(result);
}
