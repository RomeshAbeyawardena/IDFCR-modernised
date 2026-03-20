namespace IDCR.Abstractions.Interceptors;

public interface IEntityInterceptor
{
    int? OrderIndex { get; }
    bool CanIntercept(IEntityInterceptorContext context);
    void Intercept(IEntityInterceptorContext context);
    Task<bool> CanInterceptAsync(IEntityInterceptorContext context, CancellationToken cancellationToken) => Task.FromResult(CanIntercept(context));
    Task InterceptAsync(IEntityInterceptorContext context, CancellationToken cancellationToken)
    {
        Intercept(context);
        return Task.CompletedTask;
    }
}
