using System.Collections.Concurrent;
using System.Reflection;

namespace IDCR.Abstractions.Mapper;

internal class MapperConvention<TSource, TDestination> : IMapperConvention<TSource, TDestination>
{
    private readonly ConcurrentDictionary<MemberInfo, MemberInfo> _mappingDictionary = [];

    public IDictionary<MemberInfo, MemberInfo>
        Mappings => _mappingDictionary;

    public IMapperPropertyMappingConvention<TSource, TDestination> Map()
    {
        return new MapperPropertyMappingConvention<TSource, TDestination>(this);
    }
}