using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Results;
using System.Linq.Expressions;

namespace IDFCR.Abstractions.Persistence.Tests;

internal interface IPagedGlobalRequest : IPagedQuery
{
    bool ShowAll { get; set; }
}

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

internal class PagedCustomerFilter : PagedFilterBase<PagedCustomerRequest, DbCustomer>
{
    protected override Expression<Func<DbCustomer, bool>> BuildPredicate(IQueryable<DbCustomer> queryable, PagedCustomerRequest request)
    {
        var expression = base.StarterExpression;
        var innerExpression =  base.StarterExpression;

        if (!string.IsNullOrWhiteSpace(request.NameContains))
        {
            innerExpression.And(x => x.FirstName.Contains(request.NameContains));
            innerExpression.Or(x => !string.IsNullOrWhiteSpace(x.MiddleName) && x.MiddleName.Contains(request.NameContains));
            innerExpression.Or(x => x.LastName.Contains(request.NameContains));
        }

        return expression.And(innerExpression);
    }
}
