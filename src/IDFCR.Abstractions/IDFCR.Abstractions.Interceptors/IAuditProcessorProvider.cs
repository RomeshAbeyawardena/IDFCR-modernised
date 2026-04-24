using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Interceptors;

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
