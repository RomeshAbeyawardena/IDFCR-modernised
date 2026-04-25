namespace IDFCR.Abstractions.Interceptors;

//from DI or whatever 
internal class DefaultEntityInterceptorFactory(IEnumerable<IEntityInterceptor> interceptors) : IEntityInterceptorFactory
{
    public IDictionary<Type, object> SharedContextObjects { get; } = new Dictionary<Type, object>();

    public async ValueTask<IEnumerable<IEntityInterceptor>> GetEntityInterceptorsAsync(IEntityInterceptorContext context, CancellationToken cancellationToken)
    {
        var interceptorList = new List<IEntityInterceptor>();
        foreach (var interceptor in interceptors)
        {
            if (await interceptor.CanInterceptAsync(context, cancellationToken))
            {
                interceptor.Context = this;
                interceptorList.Add(interceptor);
            }
        }

        return interceptorList;
    }

    public async Task InvokeAsync(IEnumerable<IEntityInterceptor> entityInterceptors, IEntityInterceptorContext context, CancellationToken cancellationToken)
    {
        foreach (var interceptor in entityInterceptors.OrderBy(x => x.OrderIndex))
        {
            await interceptor.InterceptAsync(context, cancellationToken);
        }
    }
}
