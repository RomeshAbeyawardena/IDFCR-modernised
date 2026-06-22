using IDFCR.Abstractions.Filters.Extensions;
using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Filters.Tests.Assets;

[GlobalFilter(false, true)]
internal sealed class ConsumerPagedFilter<TRequest, TDb> : FilterBase<TRequest, TDb>, IPagedFilter<TRequest, TDb>
    where TRequest : IPagedQuery
{
    public override IQueryable<TDb> Apply(IQueryable<TDb> queryable, TRequest request)
    {
        return queryable;
    }

    (IQueryable<TDb> query, int totalEntries) IPagedFilter<TRequest, TDb>.Apply(IQueryable<TDb> queryable, TRequest request)
    {
        return (queryable, queryable.Count());
    }
}
