using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Metadata;
using Microsoft.Extensions.Options;

namespace IDFCR.Abstractions.Persistence.Interceptors;

/// <summary>
/// Intercepts delete operations and converts them into soft deletes when enabled.
/// </summary>
/// <typeparam name="TDatabaseOptions">Reserved generic type parameter preserved by the public signature.</typeparam>
public class SoftDeletionEntityInterceptor<TDatabaseOptions>(IOptions<DatabaseConfiguration> options)
    : EntityInterceptorBase(EntityContextBehaviorStage.Pre, EntityContextBehavior.Delete, 0)
    
{
    /// <inheritdoc />
    public override bool ShouldIntercept(IEntityInterceptorContext context)
    {
        return options.Value.UseSoftDeletion
            && context.Model is ISuppressable suppressable
            && !suppressable.Suppressed;
    }

    /// <inheritdoc />
    public override void Intercept(IEntityInterceptorContext context)
    {
        if (context.Model is ISuppressable suppressable)
        {
            suppressable.Suppressed = true;
            WithContext<RepositoryInterceptorContext>(context, ctx => ctx.BypassOperation = true);
        }
    }
}
