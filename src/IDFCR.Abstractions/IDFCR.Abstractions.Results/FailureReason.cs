namespace IDFCR.Abstractions.Results;

/// <summary>
/// Describes the broad reason a unit result failed.
/// </summary>
public enum FailureReason
{
    /// <summary>
    /// An unknown failure.
    /// </summary>
    Unknown = 99,

    /// <summary>
    /// No failure occurred.
    /// </summary>
    None = 0,

    /// <summary>
    /// The requested entity was not found.
    /// </summary>
    NotFound = 1,

    /// <summary>
    /// The operation conflicted with the current state.
    /// </summary>
    Conflict = 2,

    /// <summary>
    /// The operation failed validation.
    /// </summary>
    ValidationError = 3,

    /// <summary>
    /// The caller was not authorized.
    /// </summary>
    Unauthorized = 4,

    /// <summary>
    /// The caller was forbidden from performing the operation.
    /// </summary>
    Forbidden = 5,

    /// <summary>
    /// An internal error occurred.
    /// </summary>
    InternalError = 6,

    /// <summary>
    /// An external dependency error
    /// </summary>
    ExternalDependencyError = 7,

    /// <summary>
    /// An authorisation error
    /// </summary>
    AuthorizationError = 8,
    /// <summary>
    /// A not supported error, such as an unsupported media type or operation.
    /// </summary>
    NotSupported = 9
}
