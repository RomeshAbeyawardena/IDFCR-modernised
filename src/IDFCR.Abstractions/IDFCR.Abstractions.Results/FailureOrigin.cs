namespace IDFCR.Abstractions.Results;

/// <summary>
/// Represents the origin of a failure in a result. This can be used to categorize failures and provide more context about where the failure occurred, whether it was due to an internal issue, an error in the caller's code, or if the origin is unknown.
/// </summary>
public enum FailureOrigin
{
    /// <summary>
    /// An unknown failure origin indicates that the source of the failure cannot be determined. This may occur when there is insufficient information to categorize the failure or when the failure does not fit into predefined categories. It serves as a catch-all for failures that do not have a clear origin, allowing for flexibility in handling unexpected or unclassified errors.
    /// </summary>
    Unknown,
    /// <summary>
    /// Internal failures are those that originate from within the system or component being evaluated. These failures typically indicate issues such as bugs, exceptions, or other problems that arise from the internal workings of the system. Internal failures suggest that there may be a need for debugging, code review, or further investigation to identify and resolve the underlying issue.
    /// </summary>
    Internal,
    /// <summary>
    /// Caller code failures are those that originate from the code that is calling or interacting with the system or component being evaluated. These failures may indicate issues such as incorrect usage, invalid input, or other problems that arise from the caller's code. Caller code failures suggest that there may be a need for the caller to review their code, ensure proper usage of the system, and address any issues that may be causing the failure.
    /// </summary>
    CallerCode
}
