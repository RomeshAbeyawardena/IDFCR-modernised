namespace IDCR.Abstractions.Mapper;

public interface IRecordMapper<TSource> : IMapper<TSource>
{
    MappingState MappingState { get; }
}
