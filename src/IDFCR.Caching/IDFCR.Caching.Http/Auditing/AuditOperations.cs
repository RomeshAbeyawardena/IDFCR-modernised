namespace IDFCR.Caching.Http.Auditing;

/// <summary>
/// Represents the operations that can be audited in the distributed group cache.
/// </summary>
public static class AuditOperations
{
    /// <summary>
    /// Represents the "Get" operation in the distributed group cache.
    /// </summary>
    public const string Get = nameof(Get);
    /// <summary>
    /// Represents the "Set" operation in the distributed group cache.
    /// </summary>
    public const string Set = nameof(Set);
    /// <summary>
    /// Represents the "Remove" operation in the distributed group cache.
    /// </summary>
    public const string Remove = nameof(Remove);
}

/// <summary>
/// Represents the possible outcomes of audited operations in the distributed group cache.
/// </summary>
public static class AuditOutcome
{
    /// <summary>
    /// Represents a successful outcome of an audited operation in the distributed group cache.
    /// </summary>
    public const string Hit = nameof(Hit);
    /// <summary>
    /// Represents a failed outcome of an audited operation in the distributed group cache.
    /// </summary>
    public const string Miss = nameof(Miss);

    /// <summary>
    /// Represents a stored outcome of an audited operation in the distributed group cache.
    /// </summary>
    public const string Stored = nameof(Stored);
    /// <summary>
    /// Represents a removed outcome of an audited operation in the distributed group cache.
    /// </summary>
    public const string Removed = nameof(Removed);


    public const string Failed = nameof(Failed);
}