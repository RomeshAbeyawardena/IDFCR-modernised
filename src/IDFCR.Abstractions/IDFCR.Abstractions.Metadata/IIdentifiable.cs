namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents an entity that has a unique identifier of a specified type. This interface defines a property for storing the identifier, allowing for consistent handling of entities that require unique identification across different implementations. The Id property can be used to store and retrieve the unique identifier of the entity, which is essential for various operations such as data storage, retrieval, and manipulation in applications and systems that implement this interface. The generic type parameter TKey allows for flexibility in choosing the type of the identifier, such as int, Guid, or any other struct type that can serve as a unique identifier.
/// </summary>
/// <typeparam name="TKey"></typeparam>
public interface IIdentifiable<TKey>
    where TKey : struct
{
    /// <summary>
    /// Gets or sets the unique identifier of the entity. This property is used to store and retrieve the unique identifier, which is essential for various operations such as data storage, retrieval, and manipulation in applications and systems that implement this interface. The Id property should be assigned a unique value when the entity is created to ensure proper identification and tracking throughout its lifecycle. The generic type parameter TKey allows for flexibility in choosing the type of the identifier, such as int, Guid, or any other struct type that can serve as a unique identifier.
    /// </summary>
    public TKey Id { get; set; }
}
