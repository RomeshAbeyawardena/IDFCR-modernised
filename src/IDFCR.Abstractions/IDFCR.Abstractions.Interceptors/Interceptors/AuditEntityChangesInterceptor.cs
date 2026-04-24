using IDFCR.Abstractions.Metadata;

namespace IDFCR.Abstractions.Interceptors.Interceptors;

/// <summary>
/// Represents an interceptor for auditing changes to entities, allowing for the tracking of modifications and the generation of audit records based on those changes. This abstract class provides a foundation for creating custom audit interceptors by defining common properties and methods that can be overridden by derived classes. The AuditEntityChangesInterceptorBase class allows developers to specify the entity type and corresponding audit processor provider, as well as an optional entity name to identify the type of entity being audited. By inheriting from this base class, developers can easily implement custom logic for auditing changes to entities within applications and systems that require tracking of entity modifications for compliance, security, or other purposes. The interceptor is designed to be applied at the Post stage of an Update behavior, allowing it to capture changes after they have been made to the entity.
/// </summary>
/// <param name="provider">The audit processor provider to be used for auditing changes.</param>
public class AuditEntityChangesInterceptor(IAuditProcessorProvider provider) 
    : EntityInterceptorBase(EntityContextBehaviorStage.Post, EntityContextBehavior.Update, 0)
{
#pragma warning disable CS0809
    /// <summary>
    /// Defines the key used to store the old model of the entity being audited in the context data. This key is used to retrieve the previous state of the entity before changes were made, allowing for comparison with the new model to identify modifications and generate audit records accordingly. The use of this key is essential for tracking changes to entities and ensuring that accurate audit information is captured for compliance, security, or other purposes related to monitoring entity modifications within applications and systems that utilize auditing mechanisms for tracking entity modifications.
    /// </summary>
    public const string OldDataKey = "old-model";

    /// <inheritdoc />
    public override bool CanIntercept(IEntityInterceptorContext context)
    {
        return context.Data.ContainsKey(OldDataKey)
            && context.Model is IAuditable auditable
            && !string.IsNullOrWhiteSpace(auditable.AuditEntityName);
    }

    /// <summary>
    /// Synchronously intercepts the entity changes after they have been made, allowing for the auditing of modifications to the entity. This method is responsible for checking if the context contains a new model of the entity being audited, and if so, it retrieves the audit processor provider to perform the audit of changes between the old and new values of the entity. The implementation of this method can include logic for determining which properties or fields have been modified, capturing the previous and current values, and creating audit entries that can be stored or processed further for compliance, security, or other purposes related to tracking entity modifications within applications and systems that utilize auditing mechanisms for tracking entity modifications.
    /// </summary>
    /// <param name="context">The context of the entity interceptor.</param>
    [Obsolete("This method will not wait and may be cancelled if long-running.", true)]
    public override async void Intercept(IEntityInterceptorContext context)
    {
        await InterceptAsync(context, CancellationToken.None);
    }
#pragma warning restore
    /// <summary>
    /// Intercepts the entity changes after they have been made, allowing for the auditing of modifications to the entity. This method is responsible for checking if the context contains a new model of the entity being audited, and if so, it retrieves the audit processor provider to perform the audit of changes between the old and new values of the entity. The implementation of this method can include logic for determining which properties or fields have been modified, capturing the previous and current values, and creating audit entries that can be stored or processed further for compliance, security, or other purposes related to tracking entity modifications within applications and systems that utilize auditing mechanisms for tracking entity modifications.
    /// </summary>
    /// <param name="context">The context of the entity interceptor.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task InterceptAsync(IEntityInterceptorContext context, CancellationToken cancellationToken)
    {
        if (context.Data.TryGetValue(OldDataKey, out var oldModel) &&
             context.Model is IAuditable auditable
             && !string.IsNullOrWhiteSpace(auditable.AuditEntityName))
        {
            await provider.AuditChangesAsync(auditable.AuditEntityName, oldModel, context.Model, cancellationToken);
        }
    }
}