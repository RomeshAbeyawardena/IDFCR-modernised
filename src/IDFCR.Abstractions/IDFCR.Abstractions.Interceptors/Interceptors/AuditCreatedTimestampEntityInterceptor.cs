using IDFCR.Abstractions.Metadata;

namespace IDFCR.Abstractions.Interceptors.Interceptors;

/// <summary>
/// Represents an entity interceptor that automatically sets the CreatedTimestampUtc property of an entity that implements the IAuditCreatedTimestamp interface to the current UTC time when the entity is being inserted into the context. This interceptor is designed to be applied during the Pre stage of the Insert behavior, ensuring that the CreatedTimestampUtc property is populated with the correct timestamp before the entity is persisted to the data store. By using this interceptor, developers can ensure that all entities implementing IAuditCreatedTimestamp have their CreatedTimestampUtc property set consistently and accurately when they are created within applications and systems that utilize interception mechanisms for managing entity operations.
/// </summary>
/// <param name="timeProvider">The timeprovider instance</param>
public sealed class AuditCreatedTimestampEntityInterceptor(TimeProvider timeProvider)
    : EntityInterceptorBase(EntityContextBehaviorStage.Pre, EntityContextBehavior.Insert, 0)
{
    /// <summary>
    /// Determines whether the interceptor should be applied to the given entity interceptor context. This method checks if the context's model is of type IAuditCreatedTimestamp and if the CreatedTimestampUtc property is not already set (i.e., it is equal to the default value). If both conditions are met, it returns true, indicating that the interceptor should be applied to set the CreatedTimestampUtc property to the current UTC time. By implementing this logic, developers can ensure that the interceptor only applies to entities that require the CreatedTimestampUtc property to be set during insertion, preventing unnecessary updates to entities that already have a valid timestamp within applications and systems that utilize interception mechanisms for managing entity operations.
    /// </summary>
    /// <param name="context">The entity interceptor context.</param>
    /// <returns>True if the interceptor should be applied; otherwise, false.</returns>
    public override bool ShouldIntercept(IEntityInterceptorContext context)
    {
        return context.Model is IAuditCreatedTimestamp auditCreatedTimestamp
            && auditCreatedTimestamp.CreatedTimestampUtc == default;
    }

    /// <summary>
    /// Executes against the given entity interceptor context. This method checks if the context's model is of type IAuditCreatedTimestamp and, if so, sets the CreatedTimestampUtc property to the current UTC time provided by the TimeProvider instance. By implementing this logic, developers can ensure that the CreatedTimestampUtc property is automatically populated with the correct timestamp when an entity is being inserted into the context, allowing for consistent and accurate tracking of entity creation times within applications and systems that utilize interception mechanisms for managing entity operations.
    /// </summary>
    /// <param name="context">The entity interceptor context.</param>
    public override void Intercept(IEntityInterceptorContext context)
    {
        if (context.Model is IAuditCreatedTimestamp auditCreatedTimestamp)
        {
            auditCreatedTimestamp.CreatedTimestampUtc = timeProvider.GetUtcNow();
        }
    }
}
