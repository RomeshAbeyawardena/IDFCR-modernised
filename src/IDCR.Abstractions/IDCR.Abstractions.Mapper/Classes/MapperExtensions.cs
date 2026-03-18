using IDCR.Abstractions.Mapper;

namespace IDCR.Abstractions.Mapper.Classes;

public static class MapperExtensions
{
    public static TDestination? Map<TSource, TDestination>(TSource source,
        IMapperConvention<TSource, TDestination> mapperConventions,
        TDestination? destination = default,
        Func<TDestination>? instanceFactory = null, 
        ITypeCacheProvider? typeCacheProvider = null, params object[] args)
    {
        if (destination == null)
        {
            if (instanceFactory == null)
            {
                var instance = Activator
                    .CreateInstance(typeof(TDestination), args)
                    ?? throw new NullReferenceException();

                destination = (TDestination)instance;
            }
            else
                destination = instanceFactory();
        }

        if(destination == null)
        {
            throw new NullReferenceException();
        }

        typeCacheProvider ??= TypeCacheProvider.Instance;

        var sourceTypeCache = typeCacheProvider.GetTypeCache<TSource>();
        var destinationTypeCache = typeCacheProvider.GetTypeCache<TDestination>();

        foreach(var property in sourceTypeCache.Properties)
        {
            var destinationProperty = destinationTypeCache.Properties
                .FirstOrDefault(d => property.Name == d.Name);

            if(destinationProperty == null)
            {
                continue;
            }

            var value = property.GetValue(source);
            if(value == null)
            {
                continue;
            }

            destinationProperty.SetValue(destination, value);
        }

        return destination;
    }
}
