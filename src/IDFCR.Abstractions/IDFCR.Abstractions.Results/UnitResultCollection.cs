namespace IDFCR.Abstractions.Results;

/// <summary>
/// Factory helpers for collection-based unit results.
/// </summary>
public static class UnitResultCollection
{
    /// <summary>
    /// Creates a successful or failed collection result from the supplied sequence.
    /// </summary>
    /// <typeparam name="T">The collection element type.</typeparam>
    /// <param name="result">The source sequence.</param>
    /// <param name="action">The associated action.</param>
    /// <param name="isSuccess">A value indicating whether the operation succeeded.</param>
    /// <param name="namedResult">An optional name for the result.</param>
    /// <returns>A collection unit result.</returns>
    public static IUnitResultCollection<T> FromResult<T>(IEnumerable<T>? result, UnitAction action = UnitAction.Get, bool isSuccess = true, string? namedResult = null)
    {
        return new UnitResultCollection<T>(result?.ToArray(), action, isSuccess, NamedResult: namedResult);
    }

    /// <summary>
    /// Creates a failed collection result.
    /// </summary>
    /// <typeparam name="T">The collection element type.</typeparam>
    /// <param name="exception">The captured exception.</param>
    /// <param name="action">The associated action.</param>
    /// <returns>A failed collection unit result.</returns>
    public static IUnitResultCollection<T> Failed<T>(Exception exception, UnitAction action = UnitAction.None)
    {
        return new UnitResultCollection<T>(null, action, false, exception);
    }
}

/// <summary>
/// Represents a unit result whose payload is a collection.
/// </summary>
/// <typeparam name="TResult">The element type.</typeparam>
/// <param name="Result">The collection payload.</param>
/// <param name="Action">The associated action.</param>
/// <param name="IsSuccess">A value indicating whether the operation succeeded.</param>
/// <param name="Exception">The captured exception.</param>
/// <param name="FailureReason">The failure reason.</param>
/// <param name="NamedResult">An optional name for the result.</param>
internal record UnitResultCollection<TResult>(IEnumerable<TResult>? Result = null, UnitAction Action = UnitAction.Get,
    bool IsSuccess = true, Exception? Exception = null, FailureReason? FailureReason = null, string? NamedResult = null)
        : UnitResultBase<IEnumerable<TResult>>(Result, Action, IsSuccess, Exception, FailureReason, NamedResult), IUnitResultCollection<TResult>
{
}
