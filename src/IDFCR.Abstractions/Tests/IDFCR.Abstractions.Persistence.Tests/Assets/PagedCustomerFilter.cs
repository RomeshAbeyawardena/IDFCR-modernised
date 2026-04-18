using IDFCR.Abstractions.Filters;
using System.Linq.Expressions;

namespace IDFCR.Abstractions.Persistence.Tests.Assets;

internal class PagedCustomerFilter : PagedFilterBase<PagedCustomerRequest, DbCustomer>
{
    protected override Expression<Func<DbCustomer, bool>> BuildPredicate(IQueryable<DbCustomer> queryable, PagedCustomerRequest request)
    {
        var expression = base.StarterExpression;
        var innerExpression = base.StarterExpression;

        if (!string.IsNullOrWhiteSpace(request.NameContains))
        {
            innerExpression.And(x => x.FirstName.Contains(request.NameContains));
            innerExpression.Or(x => !string.IsNullOrWhiteSpace(x.MiddleName) && x.MiddleName.Contains(request.NameContains));
            innerExpression.Or(x => x.LastName.Contains(request.NameContains));
        }

        return expression.And(innerExpression);
    }
}
