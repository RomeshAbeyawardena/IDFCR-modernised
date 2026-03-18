using System.Reflection;

namespace IDCR.Abstractions.Mapper;

public interface IMapperConvention<TSource, TDestination>
{
    IDictionary<MemberInfo, MemberInfo> Mappings { get; }
    IMapperPropertyMappingConvention<TSource, TDestination> Map();
}
