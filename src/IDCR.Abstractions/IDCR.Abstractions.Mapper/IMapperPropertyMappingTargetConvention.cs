using System.Linq.Expressions;

namespace IDCR.Abstractions.Mapper;

public interface IMapperPropertyMappingTargetConvention<TSource, TDestination>
{
    IMapperPropertyMappingConvention<TSource, TDestination> From (
        Expression<Func<TSource, object>> sourceExpression);
}