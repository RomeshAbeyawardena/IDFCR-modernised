using InternalMapper;
using System.Collections.Concurrent;

namespace IDCR.Abstractions.Mapper;

public class TypeCacheProvider : ITypeCacheProvider
{
    private static Lazy<ITypeCacheProvider> _typeCacheProvider = new (new TypeCacheProvider());
    public static ITypeCacheProvider Instance => _typeCacheProvider.Value;
    private readonly ConcurrentDictionary<Type, ITypeCache> _typeCacheDictionary = [];
    public ITypeCache GetTypeCache(Type type)
    {
        return _typeCacheDictionary.GetOrAdd(type, new TypeCache(type));
    }

    public ITypeCache<T> GetTypeCache<T>()
    {
        var typeCache = GetTypeCache(typeof(T));

        var genericInstance = new TypeCache<T>(typeCache.Properties);

        return genericInstance;
    }
}
