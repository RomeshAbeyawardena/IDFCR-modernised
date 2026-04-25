using BuildTools.Infrastructure.Features.Settings;
using BuildTools.Shared.Features.Settings;
using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Results;
using IDFCR.Persistence.EntityFrameworkCore;
using IDFCR.Persistence.EntityFrameworkCore.Attributes;
using Microsoft.EntityFrameworkCore;

namespace BuildTools.Infrastructure.SqlServer.Features.Settings;

[RegisteredRepository]
public class SettingRepository(PackageManagerDbContext db, IFilterFactory filterFactory, IEntityInterceptorFactory entityInterceptorFactory)
    : EntityFrameworkRepositoryBase<PackageManagerDbContext, ISetting, SettingEntity, Setting, Guid>(db, filterFactory, entityInterceptorFactory), ISettingRepository
{
    protected override Task<Guid> OnAddAsync(SettingEntity entry, Setting rawEntry, CancellationToken cancellationToken)
    {
        entry.ModifiedTimestampUtc = entry.CreatedTimestampUtc;
        return base.OnAddAsync(entry, rawEntry, cancellationToken);
    }

    public async Task<IUnitResult<Setting>> GetSettingAsync(string key, string? type, string? environmentName, CancellationToken cancellationToken)
    {
        var query = FilterFactory.Apply(DbSet.AsNoTracking(), new GetPagedSettingsQuery
        {
            Key = key,
            Type = type,
            Environment = environmentName
        });

        var result = await query.FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            return UnitResult.NotFound<Setting>(key);
        }

        return UnitResult.FromResult(base.Map(result));
    }

    public Task<IUnitResult<Setting>> GetSettingAsync(string key, string? type, CancellationToken cancellationToken)
    {
        return GetSettingAsync(key, type, null, cancellationToken);
    }

    public async Task<IUnitResult<string>> GetValueAsync(string key, string? type, CancellationToken cancellationToken)
    {
        var settingResult = await GetSettingAsync(key, type, cancellationToken);

        if (settingResult.HasValue)
        {
            return UnitResult.FromResult(settingResult.Result.Value);
        }

        return UnitResult.Failed<string>(settingResult?.Exception ?? new InvalidOperationException("Exception unknown"));
    }

    protected override bool IsHandled(Exception exception)
    {
        return true;
    }
}
