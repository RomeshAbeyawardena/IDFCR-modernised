using BuildTools.Infrastructure.Features.Settings;
using BuildTools.Shared.Features.Settings;
using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Results;
using Microsoft.EntityFrameworkCore;

namespace BuildTools.Infrastructure.SqlServer.Features.Settings;

public class SettingRepository(PackageManagerDbContext db, IFilterFactory filterFactory, IEntityInterceptorFactory entityInterceptorFactory) 
    : EntityFrameworkRepositoryBase<PackageManagerDbContext, ISetting, SettingEntity, Setting, Guid>(db, filterFactory, entityInterceptorFactory), ISettingRepository
{
    public async Task<IUnitResult<string>> GetValueAsync(string key, string? type, CancellationToken cancellationToken)
    {
        var query = FilterFactory.Apply(DbSet, new GetPagedSettingsQuery { });

        var result = await query.FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            return UnitResult.NotFound<string>(key);
        }

        return UnitResult.FromResult(result.Value);
    }

    protected override bool IsHandled(Exception exception)
    {
        return true;
    }
}
