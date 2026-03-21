using IDFCR.Abstractions.Metadata;

namespace IDFCR.Abstractions.Interceptors.Interceptors;

public sealed class AuditModifiedTimestampEntityInterceptor(TimeProvider timeProvider) 
    : EntityInterceptorBase(EntityContextBehaviorStage.Pre, EntityContextBehavior.Update, 0)
{
    public override bool ShouldIntercept(IEntityInterceptorContext context)
    {
        return context.Model is IAuditModifiedTimestamp;
    }

    public override void Intercept(IEntityInterceptorContext context)
    {
        if (context.Model is IAuditModifiedTimestamp auditModifiedTimestamp)
        {
            auditModifiedTimestamp.ModifiedTimestampUtc = timeProvider.GetUtcNow();
        }
    }
}