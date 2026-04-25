using IDFCR.Abstractions.Mapper;

namespace BuildTools.Shared.Features.Settings.Audit;

public class SettingAudit : MapperBase<ISettingAudit>, ISettingAudit
{
    public object? Id { get; set; }
    public object? SettingId { get; set; }
    public string? ChangeDescription { get; set; }
    public string? OldValueJson { get; set; }
    public string? NewValueJson { get; set; }

    public override void Map(ISettingAudit source)
    {
        Id = source.Id;
        SettingId = source.SettingId;
        ChangeDescription = source.ChangeDescription;
        OldValueJson = source.OldValueJson;
        NewValueJson = source.NewValueJson;
    }
}
