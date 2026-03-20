namespace IDCR.Abstractions.Interceptors;

public abstract class EntityInterceptorBase(EntityContextBehaviorStage stage, EntityContextBehavior behavior, int? orderIndex = null) : IEntityInterceptor
{
    public int? OrderIndex { get; } = orderIndex;

    public virtual bool ShouldIntercept(IEntityInterceptorContext context) => true;

    public virtual bool CanIntercept(IEntityInterceptorContext context)
    {
        return context.Stage == stage
            && context.Behavior == behavior
            && ShouldIntercept(context);
    }
    public abstract void Intercept(IEntityInterceptorContext context);
}

public sealed class AuditCreatedTimestampEntityInterceptor(TimeProvider timeProvider) 
    : EntityInterceptorBase(EntityContextBehaviorStage.Pre, EntityContextBehavior.Insert, 0)
{
    public override  bool ShouldIntercept(IEntityInterceptorContext context)
    {
        return context.Model is IAuditCreatedTimestamp auditCreatedTimestamp 
            && auditCreatedTimestamp.CreatedTimestampUtc == default;
    }

    public override  void Intercept(IEntityInterceptorContext context)
    {
        if (context.Model is IAuditCreatedTimestamp auditCreatedTimestamp)
        {
            auditCreatedTimestamp.CreatedTimestampUtc = timeProvider.GetUtcNow();
        }
    }
}

public interface IAuditModifiedTimestamp
{
    DateTimeOffset? ModifiedTimestampUtc { get; set; }
}

public sealed class AuditModifiedTimestampEntityInterceptor(TimeProvider timeProvider) 
    : EntityInterceptorBase(EntityContextBehaviorStage.Pre, EntityContextBehavior.Update, 0)
{
    public override bool ShouldIntercept(IEntityInterceptorContext context)
    {
        return context.Model is IAuditModifiedTimestamp auditCreatedTimestamp
            && auditCreatedTimestamp.ModifiedTimestampUtc == default;
    }

    public override void Intercept(IEntityInterceptorContext context)
    {
        if (context.Model is IAuditModifiedTimestamp auditModifiedTimestamp)
        {
            auditModifiedTimestamp.ModifiedTimestampUtc = timeProvider.GetUtcNow();
        }
    }
}