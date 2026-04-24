using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Interceptors;


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
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the audit.</returns>
    Task<IUnitResult> AuditChangesAsync(object oldValue, object newValue, CancellationToken cancellationToken);
}