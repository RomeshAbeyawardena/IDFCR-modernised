namespace IDCR.Abstractions.Interceptors;

//from DI or whatever 
public class EntityInterceptorFactory(IEnumerable<IEntityInterceptor> interceptors) : IEntityInterceptorFactory
{
    public IEnumerable<IEntityInterceptor> GetEntityInterceptors(IEntityInterceptorContext context)
    {
        return [.. interceptors.Where(x => x.CanIntercept(context))];
    }
}
