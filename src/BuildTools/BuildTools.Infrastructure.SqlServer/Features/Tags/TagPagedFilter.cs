using BuildTools.Infrastructure.Features.Tags;
using IDFCR.Abstractions.Filters;
using System.Linq.Expressions;

namespace BuildTools.Infrastructure.SqlServer.Features.Tags;

public class TagPagedFilter : PagedFilterBase<GetPagedTagsQuery, TagEntity>
{
    protected override Expression<Func<TagEntity, bool>> BuildPredicate(IQueryable<TagEntity> queryable, GetPagedTagsQuery request)
    {
        var query = StarterExpression;
        query = StarterExpression.DefaultExpression = t => true;

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.And(x => x.Name == request.Name);
        }
        else if (!string.IsNullOrWhiteSpace(request.NameContains))
        {
            query = query.And(x => x.Name.Contains(request.NameContains));
        }

        return query;
    }
}