namespace IDFCR.Abstractions.Mapper;

public interface IRecordMapper<TSource> : IMapper<TSource>
{
    MappingState MappingState { get; }
}
