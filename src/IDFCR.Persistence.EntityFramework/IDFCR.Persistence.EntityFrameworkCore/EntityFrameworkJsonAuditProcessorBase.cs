using IDFCR.Abstractions.Interceptors.Processors;
using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Interceptors.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using IDFCR.Abstractions.Metadata;

namespace IDFCR.Persistence.EntityFrameworkCore;

/// <summary>
/// Represents a base class for processing JSON-based audits using Entity Framework, allowing for the tracking of changes to entities in a JSON format within a database context. This abstract class provides a foundation for creating custom audit processors by defining common properties and methods that can be overridden by derived classes. The EntityFrameworkJsonAuditProcessorBase class allows developers to specify the entity type and corresponding audit entity type for JSON-based audits, as well as an implementation of the AuditChangesAsync method to handle the auditing of changes to entities. By inheriting from this base class, developers can implement custom logic for auditing changes to entities based on specific requirements and use cases related to data auditing and change tracking within applications that utilize Entity Framework for data access and management.
/// </summary>
/// <typeparam name="TDbContext">The type of the Entity Framework DbContext.</typeparam>
/// <typeparam name="TEntity">The type of the entity being audited.</typeparam>
/// <typeparam name="TAuditEntity">The type of the audit entity.</typeparam>
/// <param name="entityName">The name of the entity.</param>
/// <param name="contextEntityFactory">A factory function to create the DbSet for the entity.</param>
/// <param name="timeProvider">A provider for the current time.</param>
public abstract class EntityFrameworkJsonAuditProcessorBase<TDbContext, TEntity, TAuditEntity>(string entityName, 
    Func<TDbContext, DbSet<TAuditEntity>> contextEntityFactory,
    TimeProvider timeProvider) :  AuditProcessorBase<TEntity, TAuditEntity>(entityName)
    where TDbContext : DbContext
    where TEntity : class
    where TAuditEntity : class, IJsonAudit, new()
{
    /// <summary>
    /// Looks up additional information for a given key and value, allowing for the retrieval of related data or context that may be relevant to the audit process. This method can be overridden by derived classes to provide custom logic for looking up information based on specific keys and values, enabling developers to enhance the audit logs with additional context or details that may be relevant to the changes being audited. The default implementation of this method returns a completed task with a null result, indicating that no additional information is available for the given key and value. By overriding this method, developers can implement custom lookup logic to enrich the audit logs with relevant information based on specific keys and values related to the entities being audited.
    /// </summary>
    /// <param name="key">The key for which to look up additional information.</param>
    /// <param name="value">The value associated with the key.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous lookup operation. The task result contains the additional information, or null if no information is available.</returns>
    protected virtual Task<object?> LookupAsync(string key, object value, CancellationToken cancellationToken)
    {
        return Task.FromResult<object?>(default);
    }

    /// <summary>
    /// Applies additional data to the audit entry based on the old and new values of the entity being audited. This method can be overridden by derived classes to provide custom logic for applying additional data to the audit entry, allowing developers to include relevant information or context that may be important for understanding the changes being audited. The default implementation of this method does not perform any operations, indicating that no additional data is applied to the audit entry by default. By overriding this method, developers can implement custom logic to enrich the audit entry with relevant information based on the old and new values of the entity being audited.
    /// </summary>
    /// <param name="auditEntity">The audit entity being updated.</param>
    /// <param name="oldValue">The old value of the entity being audited.</param>
    /// <param name="newValue">The new value of the entity being audited.</param>
    protected virtual void ApplyEntryData(TAuditEntity auditEntity, TEntity oldValue, TEntity newValue)
    {

    }

    /// <summary>
    /// Audits the changes between the old and new values of the entity being audited, creating an audit entry in the database context if there are changes to be recorded. This method retrieves the current DbContext from the scoped resources, generates a change description based on the differences between the old and new values, and creates a new audit entity with the relevant information, including the old and new values serialized as JSON, the change description, and a timestamp. The audit entry is then added to the database context for persistence. If there are no changes to be recorded, the method returns a successful result without adding an audit entry. By overriding this method, developers can implement custom logic for auditing changes to entities based on specific requirements and use cases related to data auditing and change tracking within applications that utilize Entity Framework for data access and management.
    /// </summary>
    /// <param name="oldValue">The old value of the entity being audited.</param>
    /// <param name="newValue">The new value of the entity being audited.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the audit operation.</returns>
    public override async Task<IUnitResult> AuditChangesAsync(TEntity oldValue, TEntity newValue, CancellationToken cancellationToken)
    {
        if (Provider!.InterceptorFactory!.ScopedResources!.TryGetScopedResource(out TDbContext? context))
        {
            var changes = await this.AuditChangeDescriptionAsync(oldValue, newValue, cancellationToken, deferredLookupAsyncAction: LookupAsync);

            if (string.IsNullOrWhiteSpace(changes))
            {
                return UnitResult.Success(UnitAction.None);
            }

            var audit = contextEntityFactory(context);

            var entity = new TAuditEntity
            {
                OldValueJson = JsonSerializer.Serialize(oldValue),
                NewValueJson = JsonSerializer.Serialize(newValue),
                ChangeDescription = changes,
                CreatedTimestampUtc = timeProvider.GetUtcNow()
            };

            ApplyEntryData(entity, oldValue, newValue);

            await audit.AddAsync(entity, cancellationToken);
        }

        return UnitResult.Success(UnitAction.Add);
    }
}
