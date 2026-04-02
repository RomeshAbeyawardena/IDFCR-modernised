namespace IDFCR.Abstractions.Mapper;

/// <summary>
/// Enumeration representing the mapping state of an object, indicating whether it has been mapped or not. This enumeration can be used to track the mapping status of objects in various contexts, such as during data transformation, object mapping, or when working with mapping frameworks. The NotMapped value indicates that the object has not been mapped, while the Mapped value indicates that the object has been successfully mapped. By using this enumeration, developers can easily manage and track the mapping state of objects within their applications and systems that involve mapping operations.
/// </summary>
public enum MappingState
{
    /// <summary>
    /// Represents the state of an object that has not been mapped. This value indicates that the object has not undergone any mapping process, and its properties or values have not been transformed or assigned to another object. When an object is in the NotMapped state, it may require mapping operations to be performed before it can be used in certain contexts, such as when working with data transfer objects, view models, or when integrating with mapping frameworks. By using the NotMapped value, developers can easily identify and manage objects that have not yet been mapped within their applications and systems that involve mapping operations.
    /// </summary>
    NotMapped = 0,
    /// <summary>
    /// Represents the state of an object that has been successfully mapped. This value indicates that the object has undergone the mapping process, and its properties or values have been transformed or assigned to another object as intended. When an object is in the Mapped state, it can be used in various contexts where the mapped data is required, such as when working with data transfer objects, view models, or when integrating with mapping frameworks. By using the Mapped value, developers can easily identify and manage objects that have been successfully mapped within their applications and systems that involve mapping operations.
    /// </summary>
    Mapped = 1
}
