namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents an object that has a name property.
/// </summary>
public interface INamed
{
    /// <summary>
    /// Gets the name of the entity.
    /// </summary>
    string Name { get; }
}
