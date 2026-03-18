using System.Linq.Expressions;

namespace IDCR.Abstractions.Mapper;

public interface IMapperPropertyMappingConvention<TSource, TDestination>
{
    IMapperPropertyMappingTargetConvention<TSource, TDestination> For (
        Expression<Func<TSource, object>> sourceExpression);
}
