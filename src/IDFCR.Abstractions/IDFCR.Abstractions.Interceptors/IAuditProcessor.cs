using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Interceptors;

public abstract class AuditProcessorBase<TEntity, TAuditEntity>(string entityName) : IAuditProcessor<TEntity, TAuditEntity>
{
    public abstract Task<IUnitResult> AuditChangesAsync(TEntity oldValue, TEntity newValue, CancellationToken cancellationToken);

    public string EntityName => entityName;

    public async Task<IUnitResult> AuditChangesAsync(object oldValue, object newValue)
    {
        if (oldValue is TEntity old && newValue is TEntity @new)
        {
            return await AuditChangesAsync(old, @new);
        }

        return UnitResult.Failed(new InvalidCastException($"Unable to cast {oldValue} to {typeof(TEntity)}"), UnitAction.None, FailureReason.ValidationError);
    }
}

public interface IAuditProcessor<TEntity, TAuditEntity> : IAuditProcessor
{
    Task<IUnitResult> AuditChangesAsync(TEntity oldValue, TEntity newValue, CancellationToken cancellationToken);
}

public interface IAuditProcessor
{
    string EntityName { get; }
    Task<IUnitResult> AuditChangesAsync(object oldValue, object newValue);
}

public interface IAuditProcessorProvider
{
    Task<IUnitResult> AuditChangesAsync(string entityName, object oldValue, object newValue, CancellationToken cancellation);
}


public abstract class AuditEntityChangesInterceptorBase<TKey>(IAuditProcessorProvider provider) 
    : EntityInterceptorBase(EntityContextBehaviorStage.Post, EntityContextBehavior.Update, 0)
    where TKey : struct
{
    public override async void Intercept(IEntityInterceptorContext context)
    {
        if (context.Data.TryGetValue("new-model", out var newModel) &&
             context.Model is IAuditable auditable
             && !string.IsNullOrWhiteSpace(auditable.AuditEntityName))
        {
            await provider.AuditChangesAsync(auditable.AuditEntityName, context.Model, newModel, CancellationToken.None);
        }
    }
}