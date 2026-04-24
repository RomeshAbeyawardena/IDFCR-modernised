using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Interceptors;

/// <summary>
/// Represents a base class for implementing audit processors that can be used to track changes to entities and generate audit records based on those changes. This abstract class provides a foundation for creating custom audit processors by defining common properties and methods that can be overridden by derived classes. The AuditProcessorBase class allows developers to specify the entity type and corresponding audit entity type, as well as an optional entity name to identify the type of entity being audited. By inheriting from this base class, developers can easily implement custom logic for auditing changes to entities within applications and systems that require tracking of entity modifications for compliance, security, or other purposes.
/// </summary>
/// <typeparam name="TEntity">The type of the entity being audited.</typeparam>
/// <typeparam name="TAuditEntity">The type of the audit entity.</typeparam>
/// <param name="entityName">The name of the entity being audited.</param>
public abstract class AuditProcessorBase<TEntity, TAuditEntity>(string entityName) : IAuditProcessor<TEntity, TAuditEntity>
{
    /// <inheritdoc />
    public abstract Task<IUnitResult> AuditChangesAsync(TEntity oldValue, TEntity newValue, CancellationToken cancellationToken);
    /// <inheritdoc />
    public string EntityName => entityName;
    /// <inheritdoc />
    public async Task<IUnitResult> AuditChangesAsync(object oldValue, object newValue)
    {
        if (oldValue is TEntity old && newValue is TEntity @new)
        {
            return await AuditChangesAsync(old, @new);
        }

        return UnitResult.Failed(new InvalidCastException($"Unable to cast {oldValue} to {typeof(TEntity)}"), UnitAction.None, FailureReason.ValidationError);
    }
}


/// <summary>
/// Represents a processor for auditing changes to entities, allowing for the tracking of modifications and the generation of audit records based on those changes. This interface defines the contract for implementing custom audit processors that can be used to monitor and record changes to entities within applications and systems that require auditing capabilities. By implementing this interface, developers can create flexible and reusable audit processors that can be integrated into applications and systems to ensure compliance, security, or other requirements related to tracking entity modifications. The IAuditProcessor interface provides a method for auditing changes between an old value and a new value of an entity, allowing for the capture of relevant information about the changes made to the entity during its lifecycle.
/// </summary>
/// <typeparam name="TEntity">The type of the entity being audited.</typeparam>
/// <typeparam name="TAuditEntity">The type of the audit entity.</typeparam>
public interface IAuditProcessor<TEntity, TAuditEntity> : IAuditProcessor
{
    /// <summary>
    /// Audits the changes between the old value and the new value of an entity, allowing for the capture of relevant information about the modifications made to the entity. This method is responsible for comparing the old and new values of the entity, identifying any differences or changes, and generating audit records based on those changes. The implementation of this method can include logic for determining which properties or fields have been modified, capturing the previous and current values, and creating audit entries that can be stored or processed further for compliance, security, or other purposes related to tracking entity modifications.
    /// </summary>
    /// <param name="oldValue">The previous state of the entity.</param>
    /// <param name="newValue">The current state of the entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the audit.</returns>
    Task<IUnitResult> AuditChangesAsync(TEntity oldValue, TEntity newValue, CancellationToken cancellationToken);
}

/// <summary>
/// Represents a processor for auditing changes to entities, allowing for the tracking of modifications and the generation of audit records based on those changes. This interface defines the contract for implementing custom audit processors that can be used to monitor and record changes to entities within applications and systems that require auditing capabilities. By implementing this interface, developers can create flexible and reusable audit processors that can be integrated into applications and systems to ensure compliance, security, or other requirements related to tracking entity modifications. The IAuditProcessor interface provides a method for auditing changes between an old value and a new value of an entity, allowing for the capture of relevant information about the changes made to the entity during its lifecycle.
/// </summary>
public interface IAuditProcessor
{
    /// <summary>
    /// Gets the name of the entity being audited. This property is used to identify the type of entity for which the audit processor is responsible. The entity name can be used in audit records or logs to indicate which type of entity was modified, providing context for the changes being tracked. By defining this property, developers can ensure that their audit processors are associated with specific entity types, allowing for more organized and meaningful audit records within applications and systems that utilize auditing mechanisms for tracking entity modifications.
    /// </summary>
    string EntityName { get; }
    /// <summary>
    /// Audits the changes between the old value and the new value of an entity, allowing for the capture of relevant information about the modifications made to the entity. This method is responsible for comparing the old and new values of the entity, identifying any differences or changes, and generating audit records based on those changes. The implementation of this method can include logic for determining which properties or fields have been modified, capturing the previous and current values, and creating audit entries that can be stored or processed further for compliance, security, or other purposes related to tracking entity modifications.
    /// </summary>
    /// <param name="oldValue">The previous state of the entity.</param>
    /// <param name="newValue">The current state of the entity.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the audit.</returns>
    Task<IUnitResult> AuditChangesAsync(object oldValue, object newValue);
}

/// <summary>
/// Represents a provider for audit processors, allowing for the retrieval and execution of audit processing logic based on the entity name and the changes being audited. This interface defines the contract for implementing a provider that can handle the auditing of changes to entities by invoking the appropriate audit processor based on the entity name and the old and new values of the entity. By implementing this interface, developers can create flexible and reusable audit processor providers that can be integrated into applications and systems to ensure that changes to entities are properly audited and recorded according to the specific requirements of the application or system. The IAuditProcessorProvider interface provides a method for auditing changes by accepting the entity name, old value, new value, and a cancellation token, allowing for asynchronous processing of audit records based on the changes made to entities within applications and systems that utilize auditing mechanisms for tracking entity modifications.
/// </summary>
public interface IAuditProcessorProvider
{
    /// <summary>
    /// Audits the changes between the old value and the new value of an entity based on the specified entity name, allowing for the capture of relevant information about the modifications made to the entity. This method is responsible for determining which audit processor to invoke based on the entity name, comparing the old and new values of the entity, identifying any differences or changes, and generating audit records based on those changes. The implementation of this method can include logic for selecting the appropriate audit processor, handling any exceptions that may occur during auditing, and ensuring that audit records are properly stored or processed further for compliance, security, or other purposes related to tracking entity modifications within applications and systems that utilize auditing mechanisms for tracking entity modifications.
    /// </summary>
    /// <param name="entityName">The name of the entity being audited.</param>
    /// <param name="oldValue">The previous state of the entity.</param>
    /// <param name="newValue">The current state of the entity.</param>
    /// <param name="cancellation">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the audit.</returns>
    Task<IUnitResult> AuditChangesAsync(string entityName, object oldValue, object newValue, CancellationToken cancellation);
}

/// <summary>
/// Represents an interceptor for auditing changes to entities, allowing for the tracking of modifications and the generation of audit records based on those changes. This abstract class provides a foundation for creating custom audit interceptors by defining common properties and methods that can be overridden by derived classes. The AuditEntityChangesInterceptorBase class allows developers to specify the entity type and corresponding audit processor provider, as well as an optional entity name to identify the type of entity being audited. By inheriting from this base class, developers can easily implement custom logic for auditing changes to entities within applications and systems that require tracking of entity modifications for compliance, security, or other purposes. The interceptor is designed to be applied at the Post stage of an Update behavior, allowing it to capture changes after they have been made to the entity.
/// </summary>
/// <param name="provider">The audit processor provider to be used for auditing changes.</param>
public abstract class AuditEntityChangesInterceptorBase(IAuditProcessorProvider provider) 
    : EntityInterceptorBase(EntityContextBehaviorStage.Post, EntityContextBehavior.Update, 0)
{
    /// <summary>
    /// Intercepts the entity changes after they have been made, allowing for the auditing of modifications to the entity. This method is responsible for checking if the context contains a new model of the entity being audited, and if so, it retrieves the audit processor provider to perform the audit of changes between the old and new values of the entity. The implementation of this method can include logic for determining which properties or fields have been modified, capturing the previous and current values, and creating audit entries that can be stored or processed further for compliance, security, or other purposes related to tracking entity modifications within applications and systems that utilize auditing mechanisms for tracking entity modifications.
    /// </summary>
    /// <param name="context">The context of the entity interceptor.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task InterceptAsync(IEntityInterceptorContext context, CancellationToken cancellationToken)
    {
        if (context.Data.TryGetValue("new-model", out var newModel) &&
             context.Model is IAuditable auditable
             && !string.IsNullOrWhiteSpace(auditable.AuditEntityName))
        {
            await provider.AuditChangesAsync(auditable.AuditEntityName, context.Model, newModel, CancellationToken.None);
        }
    }
}