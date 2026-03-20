namespace IDCR.Abstractions.Mapper.Records;

public abstract record MapperBase<TSource>() : IMapper<TSource>
{
    public abstract TSource Source { get; }

    public T? Map<T>(TSource source, params object[] parameters) where T : class, IMapper<TSource>
    {
        return Activator.CreateInstance(typeof(T), parameters) as T;
    }

    public T Map<T>(TSource source) where T : IMapper<TSource>, new()
    {
        var result = new T();
        result.Map(source);

        return result;
    }
    public abstract void Map(TSource source);

}
