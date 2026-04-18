using IDFCR.Abstractions.Metadata;

namespace IDFCR.Abstractions.Interceptors.Interceptors;

/// <summary>
/// Represents an entity interceptor that automatically sets the ModifiedTimestampUtc property of an entity that implements the IAuditModifiedTimestamp interface to the current UTC time when the entity is being updated in the context. This interceptor is designed to be applied during the Pre stage of the Update behavior, ensuring that the ModifiedTimestampUtc property is populated with the correct timestamp before the entity is persisted to the data store. By using this interceptor, developers can ensure that all entities implementing IAuditModifiedTimestamp have their ModifiedTimestampUtc property set consistently and accurately when they are updated within applications and systems that utilize interception mechanisms for managing entity operations.
/// </summary>
/// <param name="timeProvider">The timeprovider instance</param>
public sealed class AuditModifiedTimestampEntityInterceptor(TimeProvider timeProvider)
    : EntityInterceptorBase(EntityContextBehaviorStage.Pre, EntityContextBehavior.Update, 0)
{
    /// <summary>
    /// Determines whether the interceptor should be applied to the given entity interceptor context. This method checks if the context's model is of type IAuditModifiedTimestamp. If the condition is met, it returns true, indicating that the interceptor should be applied to set the ModifiedTimestampUtc property to the current UTC time. By implementing this logic, developers can ensure that the interceptor only applies to entities that require the ModifiedTimestampUtc property to be set during updates, allowing for consistent and accurate tracking of entity modification times within applications and systems that utilize interception mechanisms for managing entity operations.
    /// </summary>
    /// <param name="context">The entity interceptor context.</param>
    /// <returns>True if the interceptor should be applied; otherwise, false.</returns>
    public override bool ShouldIntercept(IEntityInterceptorContext context)
    {
        return context.Model is IAuditModifiedTimestamp;
    }

    /// <summary>
    /// Executes against the given entity interceptor context. This method checks if the context's model is of type IAuditModifiedTimestamp and, if so, sets the ModifiedTimestampUtc property to the current UTC time provided by the TimeProvider instance. By implementing this logic, developers can ensure that the ModifiedTimestampUtc property is automatically populated with the correct timestamp when an entity is being updated in the context, allowing for consistent and accurate tracking of entity modification times within applications and systems that utilize interception mechanisms for managing entity operations.
    /// </summary>
    /// <param name="context">The entity interceptor context.</param>
    public override void Intercept(IEntityInterceptorContext context)
    {
        if (context.Model is IAuditModifiedTimestamp auditModifiedTimestamp)
        {
            auditModifiedTimestamp.ModifiedTimestampUtc = timeProvider.GetUtcNow();
        }
    }
}
