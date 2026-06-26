using IDFCR.Abstractions.Metadata;

namespace IDFCR.Abstractions.Mapper.Records;

/// <summary>
/// Provides a base implementation for mapping records of a specified source type. Intended to be inherited by types
/// that implement custom record mapping logic.
/// </summary>
/// <remarks>This abstract record serves as a foundation for implementing record mappers that operate on a
/// specific source type. It manages mapping state to ensure that mapping occurs only once per instance and provides
/// utility methods for mapping using different mapper types. Derived classes must implement the MapMembers method to
/// define how individual members are mapped from the source.</remarks>
/// <typeparam name="TSource">The type of the source record to be mapped.</typeparam>
public abstract record RecordMapperBase<TSource>() : IRecordMapper<TSource>
{
    private long mappedState = 0;

    /// <summary>
    /// Gets the current instance cast to the specified source type.
    /// </summary>
    /// <remarks>Use this property to access the current object as the generic source type parameter. This is
    /// useful in scenarios where the derived class needs to expose itself as the generic type.</remarks>
    protected TSource Source
    {
        get
        {
            return (TSource)(object)this;
        }
    }

    /// <summary>
    /// Gets the current mapping state in a thread-safe manner.
    /// </summary>
    public MappingState MappingState => (MappingState)Interlocked.Read(ref mappedState);

    ///<inheritdoc />
    public T? Map<T>(params object[] parameters) where T : class, IMapper<TSource>
    {
        var instance = Activator.CreateInstance(typeof(T), parameters);

        if (instance is null || instance is not T mapper)
        {
            return null;
        }

        mapper.Map(Source);
        return mapper;
    }

    ///<inheritdoc />
    public T Map<T>() where T : IMapper<TSource>, new()
    {
        var result = new T();
        result.Map(Source);

        return result;
    }
    ///<inheritdoc />
    public virtual void Map(TSource source)
    {
        if (source is null)
        {
            return;
        }

        if (Interlocked.Exchange(ref mappedState, 1) == 1)
        {
            throw new InvalidOperationException("Mapping has already occured once with this record");
        }

        if (this is IAuditCreatedTimestamp auditCreatedTimestamp
            && source is IAuditCreatedTimestamp sourceCreatedTimestamp)
        {
            auditCreatedTimestamp.CreatedTimestampUtc = sourceCreatedTimestamp.CreatedTimestampUtc;
        }

        if (this is IAuditModifiedTimestamp auditModifiedTimestamp
                && source is IAuditModifiedTimestamp sourceModifiedTimestamp)
        {
            auditModifiedTimestamp.ModifiedTimestampUtc = sourceModifiedTimestamp.ModifiedTimestampUtc;
        }

        MapMembers(Source);
    }

    ///<inheritdoc />
    public abstract void MapMembers(TSource source);
}
