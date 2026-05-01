using IDFCR.Persistence.EntityFrameworkCore;
using IDFCR.Abstractions.Interceptors.Extensions;
using IDFCR.Abstractions.Results;

using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace BuildTools.Infrastructure.SqlServer.Features.Settings.Processors;

public class SettingAuditProcessor(TimeProvider timeProvider) 
    : EntityFrameworkJsonAuditProcessorBase<PackageManagerDbContext, SettingEntity, SettingAuditEntity>(nameof(SettingEntity), ctx => ctx.SettingAudits, timeProvider)
{
    protected async override Task<object?> LookupAsync(string key, object value, CancellationToken cancellationToken)
    {
        if (key == "Environment" && value is Guid id)
        {
            var environment = (await Context!.Environments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken));

            if (environment is not null)
            {
                return environment.DisplayName ?? environment.Name;
            }
        }

        return null;
    }

    protected override void ApplyEntryData(SettingAuditEntity auditEntity, SettingEntity oldValue, SettingEntity newValue)
    {
        auditEntity.SettingId = newValue.Id;
    }
}
