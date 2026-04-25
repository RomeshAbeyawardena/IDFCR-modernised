using BuildTools.Shared.Features.Settings.Audit;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;

namespace BuildTools.Infrastructure.SqlServer.Features.Settings;

public class SettingAuditEntity : MapperBase<ISettingAudit>, ISettingAudit, IIdentifiable<Guid>, IAuditCreatedTimestamp
{
    object? ISettingAudit.Id => Id;
    object? ISettingAudit.SettingId => SettingId;

    public Guid Id { get; set; }
    public Guid? SettingId { get; set; }
    public string? ChangeDescription { get; set; }
    public string? OldValueJson { get; set; }
    public string? NewValueJson { get; set; }
    public DateTimeOffset CreatedTimestampUtc { get; set; }

    public override void Map(ISettingAudit source)
    {
        if (source.Id is Guid id)
        {
            Id = id;
        }

        if (source.SettingId is Guid settingId)
        {
            SettingId = settingId;
        }

        ChangeDescription = source.ChangeDescription;
        OldValueJson = source.OldValueJson;
        NewValueJson = source.NewValueJson;
    }
}
