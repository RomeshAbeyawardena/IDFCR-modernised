namespace IDCR.Abstractions.Mapper;

public interface IMapper<TSource>
{
    void Map(TSource source);
}