namespace IDCR.Abstractions.Mapper;

public interface ITypeCacheProvider
{
    ITypeCache GetTypeCache(Type type);
    ITypeCache<T> GetTypeCache<T>();
}
