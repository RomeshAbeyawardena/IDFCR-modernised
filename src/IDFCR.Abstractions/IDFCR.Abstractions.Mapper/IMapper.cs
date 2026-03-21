using System.Linq.Expressions;

namespace IDFCR.Abstractions.Mapper;

public interface IMapper<TSource>
{
    T? Map<T>(TSource source, params object[] parameters)
        where T : class, IMapper<TSource>;
    T Map<T>(TSource source)
        where T : IMapper<TSource>, new();
    void Map(TSource source);
}