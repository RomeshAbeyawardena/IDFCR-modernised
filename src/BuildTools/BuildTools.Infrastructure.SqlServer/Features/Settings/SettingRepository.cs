using BuildTools.Shared.Features.Settings;
using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;
using Microsoft.EntityFrameworkCore;

namespace BuildTools.Infrastructure.SqlServer.Features.Settings;
public class SettingEntity : MapperBase<ISetting>, ISetting, IIdentifiable<Guid>
{
    public Guid Id { get; set; }
    /// <inheritdoc/>
    public string Type { get; set; } = null!;
    /// <inheritdoc/>
    public string Key { get; set; } = null!;
    /// <inheritdoc/>
    public string? Value { get; set; } = null!;
    /// <inheritdoc/>
    public DateTime LastUpdatedTimestampUtc { get; set; }
    /// <inheritdoc/>
    public override void Map(ISetting source)
    {
        Type = source.Type;
        Key = source.Key;
        Value = source.Value;
        LastUpdatedTimestampUtc = source.LastUpdatedTimestampUtc;
    }
}

public class SettingRepository(PackageManagerDbContext db, IFilterFactory filterFactory, IEntityInterceptorFactory entityInterceptorFactory) 
    : EntityFrameworkRepositoryBase<PackageManagerDbContext, ISetting, SettingEntity, Setting, Guid>(db, filterFactory, entityInterceptorFactory), ISettingRepository
{
    protected override bool IsHandled(Exception exception)
    {
        return true;
    }
}
