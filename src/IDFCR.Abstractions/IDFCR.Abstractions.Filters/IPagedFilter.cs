using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Filters;

public interface IPagedFilter<TRequest, TDb> : IFilter<TRequest, TDb>
    where TRequest : IPagedQuery
{
    new (IQueryable<TDb> query, int totalEntries) Apply(IQueryable<TDb> queryable, TRequest request);
}
