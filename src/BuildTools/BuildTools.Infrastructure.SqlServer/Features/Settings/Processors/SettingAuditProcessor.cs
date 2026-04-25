using BuildTools.Infrastructure.Features.Environments;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Interceptors.Extensions;
using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Results.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace BuildTools.Infrastructure.SqlServer.Features.Settings.Processors;

public class SettingAuditProcessor() : AuditProcessorBase<SettingEntity, SettingAuditEntity>(nameof(SettingEntity))
{
    private async Task<object?> LookupAsync(string key, object value, CancellationToken cancellationToken)
    {
        //var environmentRepository = serviceProvider.GetRequiredService<IEnvironmentRepository>();
        //if (key == "Environment" && value is Guid id)
        //{
        //    var environment = (await environmentRepository.FindAsync(id, cancellationToken)).GetResultOrDefault();

        //    if (environment is not null)
        //    {
        //        return environment.DisplayName ?? environment.Name;
        //    }
        //}

        return null;
    }

    public override async Task<IUnitResult> AuditChangesAsync(SettingEntity oldValue, SettingEntity newValue, CancellationToken cancellationToken)
    {
        Provider.InterceptorFactory.SharedContextObjects.TryGetValue(typeof(PackageManagerDbContext), out var context);
        //var context = scope.ServiceProvider.GetRequiredService<PackageManagerDbContext>();
        //var changes = await this.AuditChangeDescriptionAsync(oldValue, newValue, cancellationToken, deferredLookupAsyncAction: LookupAsync);
        //context.SettingAudits.Add(new SettingAuditEntity
        //{
        //    OldValueJson = JsonSerializer.Serialize(oldValue),
        //    NewValueJson = JsonSerializer.Serialize(newValue),
        //    SettingId = newValue.Id,
        //    ChangeDescription = changes
        //});

        return UnitResult.Success(UnitAction.Add);
    }
}
