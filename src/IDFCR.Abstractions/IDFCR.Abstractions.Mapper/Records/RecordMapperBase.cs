namespace IDFCR.Abstractions.Mapper.Records;

public abstract record RecordMapperBase<TSource>() : IRecordMapper<TSource>
{
    private long mappedState = 0;

    public MappingState MappingState => (MappingState)Interlocked.Read(ref mappedState);

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

    public virtual void Map(TSource source)
    {
        if (Interlocked.Exchange(ref mappedState, 1) == 1)
        {
            throw new InvalidOperationException("Mapping has already occured once with this record");
        }

        MapMembers(source);
    }

    public abstract void MapMembers(TSource source);
}