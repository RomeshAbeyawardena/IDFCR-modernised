namespace IDFCR.Abstractions.Interceptors;

public abstract class EntityInterceptorBase(EntityContextBehaviorStage stage, EntityContextBehavior behavior, int? orderIndex = null) : IEntityInterceptor
{
    protected static void WithContext<TEntityInterceptorContext>(IEntityInterceptorContext context,
        Action<TEntityInterceptorContext> apply)
        where TEntityInterceptorContext : IEntityInterceptorContext
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        if (context is TEntityInterceptorContext interceptorContext)
        {
            apply(interceptorContext);
        }
    }

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
