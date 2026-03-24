namespace IDFCR.Abstractions.Results;

/// <summary>
/// Describes the action associated with a unit result.
/// </summary>
public enum UnitAction
{
    /// <summary>
    /// No action is associated with the result.
    /// </summary>
    None = 0,

    /// <summary>
    /// The result represents an add operation.
    /// </summary>
    Add = 1,

    /// <summary>
    /// The result represents a get operation.
    /// </summary>
    Get = 2,

    /// <summary>
    /// The result represents an update operation.
    /// </summary>
    Update = 3,

    /// <summary>
    /// The result represents a delete operation.
    /// </summary>
    Delete = 4,

    /// <summary>
    /// The result represents a pending operation.
    /// </summary>
    Pending = 5,

    /// <summary>
    /// The result represents a conflict.
    /// </summary>
    Conflict = 6
}
