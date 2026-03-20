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