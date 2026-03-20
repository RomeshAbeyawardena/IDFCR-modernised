namespace IDCR.Abstractions.Mapper;

public abstract class MapperBase<TSource>() : IMapper<TSource>
{
    public abstract TSource Source { get; }
    public T Map<T>(TSource source) where T : IMapper<TSource>, new()
    {
        var result = new T();
        result.Map(source);

        return result;
    }
    public abstract void Map(TSource source);
}