using BuildTools.Shared.Features.Settings;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Interceptors.Extensions;
using IDFCR.Abstractions.Results;

namespace BuildTools.Infrastructure.SqlServer.Features.Settings.Processors;

public class SettingAuditProcessor(PackageManagerDbContext context) : AuditProcessorBase<SettingEntity, SettingAuditEntity>(nameof(Setting))
{
    public override Task<IUnitResult> AuditChangesAsync(SettingEntity oldValue, SettingEntity newValue, CancellationToken cancellationToken)
    {
        var changes = this.AuditChanges(oldValue, newValue);
        context.SettingAudits.Add(new SettingAuditEntity
        {
            SettingId = newValue.Id,
            ChangeDescription = changes
        });

    }
}
