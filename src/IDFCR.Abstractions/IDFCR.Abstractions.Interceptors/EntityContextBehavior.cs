namespace IDFCR.Abstractions.Interceptors;

/// <summary>
/// Enumerates the different behaviors that can be applied to an entity context during interception. This enumeration defines the possible actions that can be performed on an entity, such as inserting a new entity, updating an existing entity, or deleting an entity. The EntityContextBehavior enum can be used in various scenarios where interception logic needs to determine the appropriate behavior to apply based on the context of the operation being performed on the entity. By using this enumeration, developers can implement consistent and clear handling of different entity operations within their applications and systems that utilize interception mechanisms.
/// </summary>
[Flags]
public enum EntityContextBehavior
{
    /// <summary>
    /// Represents the behavior of no specific action being applied to the entity context. When this behavior is applied, it indicates that no particular operation or modification is being performed on the entity during interception. This may be used as a default or fallback behavior when there are no specific actions to be taken on the entity, allowing the interception logic to proceed without any modifications or special handling for the entity context. The None behavior is typically used when there is a need to indicate that no specific behavior is being applied to the entity context within an application or system that utilizes interception mechanisms. 
    /// </summary>
    [Obsolete("The None behavior is deprecated and should not be used. It is recommended to use specific behaviors such as Insert, Update, or Delete to indicate the intended operation on the entity context during interception.")]
    None = 0,
    /// <summary>
    /// Represents the behavior of inserting a new entity into the context. When this behavior is applied, it indicates that a new entity is being added to the context and should be treated as such during interception. This may involve setting default values, validating the entity's properties, or performing any necessary actions to prepare the entity for insertion into the data store. The Insert behavior is typically used when creating new records or entities within an application or system that utilizes interception mechanisms.
    /// </summary>
    Insert = 1,
    /// <summary>
    /// Represents the behavior of updating an existing entity within the context. When this behavior is applied, it indicates that an existing entity is being modified and should be treated as such during interception. This may involve validating the changes being made to the entity, checking for concurrency issues, or performing any necessary actions to ensure that the update operation is handled correctly. The Update behavior is typically used when modifying existing records or entities within an application or system that utilizes interception mechanisms.
    /// </summary>
    Update = 2,
    /// <summary>
    /// Represents the behavior of deleting an existing entity from the context. When this behavior is applied, it indicates that an existing entity is being removed and should be treated as such during interception. This may involve validating the deletion operation, checking for dependencies or constraints, or performing any necessary actions to ensure that the delete operation is handled correctly. The Delete behavior is typically used when removing existing records or entities within an application or system that utilizes interception mechanisms.
    /// </summary>
    Delete = 4,
}
