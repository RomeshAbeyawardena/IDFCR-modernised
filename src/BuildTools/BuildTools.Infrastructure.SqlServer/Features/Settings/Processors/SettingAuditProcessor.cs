using BuildTools.Shared.Features.Settings;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Results;

namespace BuildTools.Infrastructure.SqlServer.Features.Settings.Processors;

public class SettingAuditProcessor() : AuditProcessorBase<SettingEntity, SettingAuditEntity>(nameof(Setting))
{
    public override Task<IUnitResult> AuditChangesAsync(SettingEntity oldValue, SettingEntity newValue, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
