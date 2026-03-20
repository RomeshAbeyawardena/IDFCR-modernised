namespace IDCR.Abstractions.Interceptors;

public interface IEntityInterceptorFactory
{
    IEnumerable<IEntityInterceptor> GetEntityInterceptors(IEntityInterceptorContext context);
}
