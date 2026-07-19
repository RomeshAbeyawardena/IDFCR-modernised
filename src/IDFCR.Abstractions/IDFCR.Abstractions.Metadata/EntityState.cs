namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents the considered state of an entity, indicating whether it is new, being updated, or in an invalid state.
/// </summary>
public enum EntityState
{
    /// <summary>
    /// Indicates that the entity is considered new and has not been persisted yet.
    /// </summary>
    New,
    /// <summary>
    /// Indicates that the entity is considered to be in an update state and has already been persisted.
    /// </summary>
    Update,
    /// <summary>
    /// Indicates that the entity is considered to be in an invalid state, which may occur if the entity's identifier is not valid or if it cannot be determined whether the entity is new or being updated.
    /// </summary>
    Invalid
}
