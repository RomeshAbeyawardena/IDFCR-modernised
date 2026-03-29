using BuildTools.Infrastructure.Features.Settings;
using IDFCR.Abstractions.Filters;
using System.Linq.Expressions;

namespace BuildTools.Infrastructure.SqlServer.Features.Settings;

public class SettingPagedFilter : PagedFilterBase<GetPagedSettingsQuery, SettingEntity>
{
    protected override Expression<Func<SettingEntity, bool>> BuildPredicate(IQueryable<SettingEntity> queryable, GetPagedSettingsQuery request)
    {
        var query = base.StarterExpression;
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.And(x => x.Key == request.Name);
        }
        else if (!string.IsNullOrWhiteSpace(request.NameContains))
        {
            query = query.And(x => x.Key.Contains(request.Name));
        }

        if (!string.IsNullOrWhiteSpace(request.Type))
        {
            query = query.And(x => x.Type == request.Type);
        }

        return query;
    }
}
