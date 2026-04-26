using IDFCR.Abstractions.Interceptors.Processors;
using IDFCR.Abstractions.Interceptors.Extensions;
using IDFCR.Abstractions.Results;

using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace BuildTools.Infrastructure.SqlServer.Features.Settings.Processors;

public class SettingAuditProcessor() : AuditProcessorBase<SettingEntity, SettingAuditEntity>(nameof(SettingEntity))
{
    private PackageManagerDbContext? context;

    private async Task<object?> LookupAsync(string key, object value, CancellationToken cancellationToken)
    {
        if (key == "Environment" && value is Guid id)
        {
            var environment = (await context!.Environments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken));

            if (environment is not null)
            {
                return environment.DisplayName ?? environment.Name;
            }
        }

        return null;
    }

    public override async Task<IUnitResult> AuditChangesAsync(SettingEntity oldValue, SettingEntity newValue, CancellationToken cancellationToken)
    {
        if (Provider!.InterceptorFactory!.ScopedResources.TryGetScopedResource(out context))
        {
            var changes = await this.AuditChangeDescriptionAsync(oldValue, newValue, cancellationToken, deferredLookupAsyncAction: LookupAsync);

            if (string.IsNullOrWhiteSpace(changes))
            {
                return UnitResult.Success(UnitAction.Add);
            }

            context.SettingAudits.Add(new SettingAuditEntity
            {
                OldValueJson = JsonSerializer.Serialize(oldValue),
                NewValueJson = JsonSerializer.Serialize(newValue),
                SettingId = newValue.Id,
                ChangeDescription = changes
            });
        }
        
        return UnitResult.Success(UnitAction.Add);
    }
}
