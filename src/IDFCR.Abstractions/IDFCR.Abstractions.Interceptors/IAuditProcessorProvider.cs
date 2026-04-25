using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Interceptors;

/// <summary>
/// Represents a provider for audit processors, allowing for the retrieval and execution of audit processing logic based on the entity name and the changes being audited. This interface defines the contract for implementing a provider that can handle the auditing of changes to entities by invoking the appropriate audit processor based on the entity name and the old and new values of the entity. By implementing this interface, developers can create flexible and reusable audit processor providers that can be integrated into applications and systems to ensure that changes to entities are properly audited and recorded according to the specific requirements of the application or system. The IAuditProcessorProvider interface provides a method for auditing changes by accepting the entity name, old value, new value, and a cancellation token, allowing for asynchronous processing of audit records based on the changes made to entities within applications and systems that utilize auditing mechanisms for tracking entity modifications.
/// </summary>
public interface IAuditProcessorProvider
{
    /// <summary>
    /// Gets or sets the entity interceptor factory associated with this audit processor provider. The interceptor factory is responsible for creating instances of entity interceptors that can be used to intercept and process entity operations, such as inserts, updates, or deletes. By accessing the interceptor factory, developers can ensure that their audit processor provider is properly integrated into the interception framework and can leverage any shared resources or services provided by the factory when performing audit operations within applications and systems that utilize interception mechanisms for managing entity operations.
    /// </summary>
    IEntityInterceptorFactory? InterceptorFactory { get; set; }
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

internal class DefaultAuditProcessorProvider(IEnumerable<IAuditProcessor> auditProcessors) : IAuditProcessorProvider
{
    public IEntityInterceptorFactory? InterceptorFactory { get; set; }

    public async Task<IUnitResult> AuditChangesAsync(string entityName, object oldValue, object newValue, CancellationToken cancellationToken)
    {
        IAuditProcessor[] processors = [.. auditProcessors.Where(x => x.EntityName == entityName)];
        if (processors.Length > 1)
        {
            return UnitResult.Failed(new InvalidOperationException($"Multiple {nameof(IAuditProcessor)}'s found for '{entityName}'"), UnitAction.Get, FailureReason.Conflict);
        }

        if (processors.Length < 1)
        {
            //nothing to process and there are no issues with this.
            return UnitResult.Success(UnitAction.None);
        }
        processors[0].Provider = this;
        return await processors[0].AuditChangesAsync(oldValue, newValue, cancellationToken);
    }
}