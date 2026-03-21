using IDFCR.Abstractions.Metadata;

namespace IDFCR.Abstractions.Interceptors.Interceptors;

public sealed class AuditCreatedTimestampEntityInterceptor(TimeProvider timeProvider) 
    : EntityInterceptorBase(EntityContextBehaviorStage.Pre, EntityContextBehavior.Insert, 0)
{
    public override  bool ShouldIntercept(IEntityInterceptorContext context)
    {
        return context.Model is IAuditCreatedTimestamp auditCreatedTimestamp 
            && auditCreatedTimestamp.CreatedTimestampUtc == default;
    }

    public override void Intercept(IEntityInterceptorContext context)
    {
        if (context.Model is IAuditCreatedTimestamp auditCreatedTimestamp)
        {
            auditCreatedTimestamp.CreatedTimestampUtc = timeProvider.GetUtcNow();
        }
    }
}
