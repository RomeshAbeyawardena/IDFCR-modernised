namespace IDFCR.Caching.Http.Auditing;

/// <summary>
/// Represents the possible outcomes of audited operations in the distributed group cache.
/// </summary>
public static class AuditOutcome
{
    /// <summary>
    /// Defines a successful outcome of an audited operation in the distributed group cache.
    /// </summary>
    public const string Hit = nameof(Hit);
    /// <summary>
    /// Defines a failed outcome of an audited operation in the distributed group cache.
    /// </summary>
    public const string Miss = nameof(Miss);

    /// <summary>
    /// Defines a stored outcome of an audited operation in the distributed group cache.
    /// </summary>
    public const string Stored = nameof(Stored);
    /// <summary>
    /// Defines a removed outcome of an audited operation in the distributed group cache.
    /// </summary>
    public const string Removed = nameof(Removed);

    /// <summary>
    /// Defines a failed outcome of an audited operation in the distributed group cache.
    /// </summary>
    public const string Failed = nameof(Failed);
}