namespace IDFCR.Abstractions.Mapper;

public abstract class MapperBase<TSource>() : IMapper<TSource>
{
    public T? Map<T>(TSource source, params object[] parameters) where T : class, IMapper<TSource>
    {
        var instance = Activator.CreateInstance(typeof(T), parameters);

        if (instance is null || instance is not T mapper)
        {
            return null;
        }

        mapper.Map(source);
        return mapper;
    }

    public T Map<T>(TSource source) where T : IMapper<TSource>, new()
    {
        var result = new T();
        result.Map(source);

        return result;
    }
    public abstract void Map(TSource source);
}