namespace IDFCR.Abstractions.Interceptors;

public interface IEntityInterceptorFactory
{
    ValueTask<IEnumerable<IEntityInterceptor>> GetEntityInterceptorsAsync(IEntityInterceptorContext context, CancellationToken cancellationToken);
    Task InvokeAsync(IEnumerable<IEntityInterceptor> entityInterceptors, IEntityInterceptorContext context, CancellationToken cancellationToken);
}
