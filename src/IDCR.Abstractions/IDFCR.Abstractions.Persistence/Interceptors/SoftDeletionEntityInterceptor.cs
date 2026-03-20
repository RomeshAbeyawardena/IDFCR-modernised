using IDCR.Abstractions.Interceptors;
using IDCR.Abstractions.Metadata;
using Microsoft.Extensions.Options;

namespace IDCR.Abstractions.Persistence.Interceptors;

public class SoftDeletionEntityInterceptor<TDatabaseOptions>(IOptions<DatabaseConfiguration> options)
    : EntityInterceptorBase(EntityContextBehaviorStage.Pre, EntityContextBehavior.Delete, 0)
    
{
    public override bool ShouldIntercept(IEntityInterceptorContext context)
    {
        return options.Value.UseSoftDeletion
            && context.Model is ISuppressable suppressable
            && !suppressable.Suppressed;
    }

    public override void Intercept(IEntityInterceptorContext context)
    {
        if (context.Model is ISuppressable suppressable)
        {
            suppressable.Suppressed = true;
            WithContext<RepositoryInterceptorContext>(context, ctx => ctx.BypassOperation = true);
        }
    }
}