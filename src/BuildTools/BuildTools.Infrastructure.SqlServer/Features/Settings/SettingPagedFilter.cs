using BuildTools.Infrastructure.Features.Settings;
using IDFCR.Abstractions.Filters;
using System.Linq.Expressions;

namespace BuildTools.Infrastructure.SqlServer.Features.Settings;

public class SettingPagedFilter : PagedFilterBase<GetPagedSettingsQuery, SettingEntity>
{
    protected override Expression<Func<SettingEntity, bool>> BuildPredicate(IQueryable<SettingEntity> queryable, GetPagedSettingsQuery request)
    {
        var query = base.StarterExpression;
        
        query.DefaultExpression = x => true;
        if (!string.IsNullOrWhiteSpace(request.Key))
        {
            query = query.And(x => x.Key == request.Key);
        }
        else if (!string.IsNullOrWhiteSpace(request.KeyContains))
        {
            query = query.And(x => x.Key.Contains(request.KeyContains));
        }

        if (!string.IsNullOrWhiteSpace(request.Type))
        {
            query = query.And(x => x.Type == request.Type);
        }

        return query;
    }
}
