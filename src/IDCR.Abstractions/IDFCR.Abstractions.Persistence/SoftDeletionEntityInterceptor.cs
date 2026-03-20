using IDCR.Abstractions.Interceptors;
using Microsoft.Extensions.Options;

namespace IDCR.Abstractions.Persistence;

public interface ISuppressable
{
    bool Suppressed { get; set; }
}

public class SoftDeletionEntityInterceptor(IOptions<DatabaseConfiguration> options)
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