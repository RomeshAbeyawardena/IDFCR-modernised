namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents an entity that can be marked as suppressed. This interface defines a property for indicating whether the entity is currently suppressed, allowing for consistent handling of suppression status across different implementations. The Suppressed property can be used to control the visibility or behavior of the entity in various contexts, such as filtering out suppressed entities from results or applying specific logic based on the suppression status. Implementing this interface allows for a standardized way to manage and track the suppression state of entities in applications and systems that require such functionality.
/// </summary>
public interface ISuppressable
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is currently suppressed. This property is used to control the visibility or behavior of the entity in various contexts, such as filtering out suppressed entities from results or applying specific logic based on the suppression status. When Suppressed is set to true, it indicates that the entity is currently suppressed and may be treated differently in certain operations or contexts. When set to false, it indicates that the entity is not suppressed and should be handled normally. Implementing this property allows for a standardized way to manage and track the suppression state of entities in applications and systems that require such functionality.
    /// </summary>
    bool Suppressed { get; set; }
}
