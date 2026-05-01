using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;

namespace BuildTools.Shared.Features.Settings.Audit;

public interface ISettingAudit : IMapper<ISettingAudit>, IJsonAudit
{
    object? Id { get; }
    object? SettingId { get; }
}
