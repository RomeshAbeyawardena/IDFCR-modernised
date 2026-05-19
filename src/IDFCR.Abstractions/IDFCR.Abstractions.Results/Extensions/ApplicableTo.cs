namespace IDFCR.Abstractions.Results.Extensions;

/// <summary>
/// Defines the applicability of a condition to a collection of results, indicating whether the condition should be applied to all results or any result in the collection. This enumeration is used to specify how a condition should be evaluated when checking for the presence of certain characteristics or values within a collection of unit results, allowing for flexible and customizable result evaluation based on the desired criteria.
/// </summary>
public enum ApplicableTo
{
    /// <summary>
    /// Defines that a condition is not applicable to any results in the collection. This value indicates that the condition should not be applied when evaluating the results, and it may be used as a default or placeholder value when no specific applicability is intended. When this value is used, it typically means that the condition will not influence the evaluation of the results, and the results will be considered without regard to the condition being checked.
    /// </summary>
    [Obsolete("Must use All or Any")]
    None,
    /// <summary>
    /// Defines that a condition is applicable to all results in the collection. This value indicates that the condition should be applied when evaluating each result, and the evaluation will only be considered successful if all results meet the condition.
    /// </summary>
    All,
    /// <summary>
    /// Defines that a condition is applicable to any result in the collection. This value indicates that the condition should be applied when evaluating each result, and the evaluation will be considered successful if at least one result meets the condition.
    /// </summary>
    Any
}
