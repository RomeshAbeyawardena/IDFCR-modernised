using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Filters.Extensions;
using IDFCR.Abstractions.Metadata;
using System.Linq.Expressions;

namespace IDFCR.Abstractions.Persistence.Tests.Assets;

[GlobalFilter]
internal class PagedGlobalFilter<TRequest, TDb> : PagedFilterBase<TRequest, TDb>
    where TRequest : IPagedGlobalRequest
    where TDb : ISuppressable
{
    protected override Expression<Func<TDb, bool>> BuildPredicate(IQueryable<TDb> queryable, TRequest request)
    {
        var expression = base.StarterExpression;

        if (!request.ShowAll)
        {
            expression = expression.And(x => !x.Suppressed);
        }

        return expression;
    }
}
