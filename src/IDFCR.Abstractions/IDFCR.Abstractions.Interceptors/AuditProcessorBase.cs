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
    public IAuditProcessorProvider? Provider { get; set; }

    /// <inheritdoc />
    public abstract Task<IUnitResult> AuditChangesAsync(TEntity oldValue, TEntity newValue, CancellationToken cancellationToken);
    /// <inheritdoc />
    public string EntityName => entityName;
    /// <inheritdoc />
    public async Task<IUnitResult> AuditChangesAsync(object oldValue, object newValue, CancellationToken cancellationToken)
    {
        if (oldValue is TEntity old && newValue is TEntity @new)
        {
            return await AuditChangesAsync(old, @new, cancellationToken);
        }

        return UnitResult.Failed(new InvalidCastException($"Unable to cast {oldValue.GetType()} to {typeof(TEntity)}"), UnitAction.None, FailureReason.ValidationError);
    }
}
