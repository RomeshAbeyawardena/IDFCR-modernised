namespace IDFCR.Abstractions.Interceptors;

/// <summary>
/// Enumerates the stages of behavior that can be applied to an entity context during interception. This enumeration defines the possible stages at which interception logic can be applied, such as before the main operation is executed (Pre) or after the main operation has been completed (Post). The EntityContextBehaviorStage enum can be used in various scenarios where interception logic needs to determine the appropriate stage to apply based on the context of the operation being performed on the entity. By using this enumeration, developers can implement consistent and clear handling of different stages of entity operations within their applications and systems that utilize interception mechanisms.
/// </summary>
public enum EntityContextBehaviorStage
{
    /// <summary>
    /// Represents the stage of behavior that occurs before the main operation is executed. When this stage is applied, it indicates that the interception logic should be executed prior to the main operation being performed on the entity. This may involve validating input data, setting default values, or performing any necessary actions to prepare for the main operation. The Pre stage is typically used when there is a need to perform actions or checks before the main operation takes place within an application or system that utilizes interception mechanisms.
    /// </summary>
    Pre,
    /// <summary>
    /// Represents the stage of behavior that occurs after the main operation has been completed. When this stage is applied, it indicates that the interception logic should be executed after the main operation has been performed on the entity. This may involve validating the results of the main operation, performing cleanup actions, or applying any necessary post-processing logic. The Post stage is typically used when there is a need to perform actions or checks after the main operation has taken place within an application or system that utilizes interception mechanisms.
    /// </summary>
    Post
}
