namespace IDCR.Abstractions.Interceptors;

public class AuditTimestampEntityInterceptor(TimeProvider timeProvider) : IEntityInterceptor
{
    public int? OrderIndex { get; } = 0;

    public bool CanIntercept(IEntityInterceptorContext context)
    {
        return context.Stage == EntityContextBehaviorStage.Pre
            && context.Behavior == EntityContextBehavior.Insert
            && context.Model is IAuditCreatedTimestamp auditCreatedTimestamp && auditCreatedTimestamp.CreatedTimestampUtc == default;
    }

    public void Intercept(IEntityInterceptorContext context)
    {
        //we don't know when this will be called and if the state of the object changed so we might as well grab the fresh state
        if (context.Model is IAuditCreatedTimestamp auditCreatedTimestamp)
        {
            auditCreatedTimestamp.CreatedTimestampUtc = timeProvider.GetUtcNow();
        }
    }
}