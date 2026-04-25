using BuildTools.Shared.Features.Audits;
using IDFCR.Abstractions.Mapper;

namespace BuildTools.Shared.Features.Settings.Audit;

public interface ISettingAudit : IMapper<ISettingAudit>, IJsonAudit
{
    object? Id { get; }
    object? SettingId { get; }
}
