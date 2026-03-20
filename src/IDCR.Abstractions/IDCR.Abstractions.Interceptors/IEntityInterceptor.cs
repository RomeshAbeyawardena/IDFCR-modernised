namespace IDCR.Abstractions.Interceptors;

public interface IEntityInterceptor
{
    int? OrderIndex { get; }
    bool CanIntercept(IEntityInterceptorContext context);
    void Intercept(IEntityInterceptorContext context);
    Task<bool> CanInterceptAsync(IEntityInterceptorContext context) => Task.FromResult(CanIntercept(context));
    Task InterceptAsync(IEntityInterceptorContext context)
    {
        Intercept(context);
        return Task.CompletedTask;
    }
}
