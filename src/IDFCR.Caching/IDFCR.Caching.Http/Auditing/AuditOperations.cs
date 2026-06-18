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
