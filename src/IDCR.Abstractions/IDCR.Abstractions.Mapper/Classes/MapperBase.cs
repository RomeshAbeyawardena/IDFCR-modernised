namespace IDCR.Abstractions.Mapper;

public abstract class MapperBase<TTarget, TSource>() : IMapper<TSource>
{
    private TTarget? _target;
    private readonly IMapperConvention<TSource, TTarget> _mapperConvention 
        = new MapperConvention<TSource, TTarget>();

    protected IMapperConvention<TSource, TTarget> Conventions => _mapperConvention;
    protected TTarget Target { set =>  _target = value; }

    public void Map(TSource source)
    {
        MapperExtensions.Map(source, _mapperConvention,  _target);
    }
}