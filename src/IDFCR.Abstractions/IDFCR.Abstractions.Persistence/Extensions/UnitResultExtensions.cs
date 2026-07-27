using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Persistence.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="IUnitResult"/> interface.
/// </summary>
public static class UnitResultExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="IUnitResult"/> indicates that there were no changes made during an update operation.
    /// </summary>
    /// <param name="result">The unit result to check.</param>
    /// <returns><c>true</c> if there were no changes; otherwise, <c>false</c>.</returns>
    public static bool HasNoChanges(this IUnitResult result)
    {
        return (!result.IsSuccess 
            && result.Action == UnitAction.Update
            && result.FailureReason == FailureReason.Conflict
            && result.Exception is not null
            && result.Exception.Message == ErrorMessages.HasNoChanges);
    }
}
