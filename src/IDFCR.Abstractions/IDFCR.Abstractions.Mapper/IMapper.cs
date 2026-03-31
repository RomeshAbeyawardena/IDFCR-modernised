using System.Linq.Expressions;

namespace IDFCR.Abstractions.Mapper;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TSource"></typeparam>
public interface IMapper<TSource>
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameters"></param>
    /// <returns></returns>
    T? Map<T>(params object[] parameters)
        where T : class, IMapper<TSource>;
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T Map<T>()
        where T : IMapper<TSource>, new();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    void Map(TSource source);
}
