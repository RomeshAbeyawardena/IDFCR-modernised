namespace IDFCR.Abstractions.Persistence;

/// <summary>
/// Contains error messages used in the persistence layer of the application.
/// </summary>
public static class ErrorMessages
{
    /// <summary>
    /// Indicates that the mapping operation failed.
    /// </summary>
    public const string MappingFailure = "Unable to map";
    /// <summary>
    /// Indicates that the entity has no changes to update.
    /// </summary>
    public const string HasNoChanges = "Has no changes";
}
