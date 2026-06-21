using IDFCR.Abstractions.Filters.Extensions;
using IDFCR.Abstractions.Filters.V2;
using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Filters;

[GlobalFilter(false, true)]
internal class DefaultPagedFilter<TRequest, TDb>(IFilterFactory filterFactory) : PagedFilterBase<TRequest, TDb>
    where TRequest : IPagedQuery
{
    public override IQueryable<TDb> Apply(IQueryable<TDb> queryable, TRequest request)
    {
        return filterFactory.Apply(queryable, request);
    }
}
