namespace IDCR.Abstractions.Mapper;

public record Mapper<TSource> : IMapper<TSource>
{
    public void Map(TSource source)
    {
        throw new NotImplementedException();
    }
}
