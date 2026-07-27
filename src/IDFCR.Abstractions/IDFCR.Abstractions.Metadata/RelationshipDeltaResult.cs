namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents a delta (change) in a list of strings, containing items to add and remove.
/// </summary>
/// <param name="EntitiesCreated">The number of entities that were created.</param>
/// <param name="RelationshipsAdded">The number of relationships that were added.</param>
/// <param name="RelationshipsRemoved">The number of relationships that were removed.</param>
public sealed record RelationshipDeltaResult(
    int EntitiesCreated,
    int RelationshipsAdded,
    int RelationshipsRemoved);
