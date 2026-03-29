using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Cli.Operations;
using BuildTools.Infrastructure.Features.Settings;

namespace BuildTools.Cli.Features.Settings;

[FeatureCommand(SettingsRootOperation.Prefix, CommandName)]
public class SettingsReadOperation(IServiceProvider serviceProvider, IManagedStream managedStream, ISettingRepository settingRepository) 
    : InjectableCommandOperationBase<SettingsReadOperation>(serviceProvider, SettingsRootOperation.Prefix, CommandName, typeof(SettingsRootOperation))
{
    public const string CommandName = "read";
    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var key = await this.GetOptionalField(managedStream, command, cancellationToken, false, "key");
        var type = await this.GetOptionalField(managedStream, command, cancellationToken, true, "type");
        var outputType = await this.GetOptionalField(managedStream, command, cancellationToken, true, "output-type");

        if (string.IsNullOrWhiteSpace(key))
        {
            //generate paged list
            var pagedResult = await settingRepository.GetPagedAsync(new GetPagedSettingsQuery 
            {
                PageSize = 20,
                Key = key,
                Type = type
            }, cancellationToken);

            if (pagedResult.HasValue)
            {
                await managedStream.DisplayPagedTable(pagedResult, t => t.Map<SettingDto>(t), cancellationToken, 
                    new TableField<SettingDto> { Field = s => s.Key, Title = "Setting Key", RowWidth = 20 },
                    new TableField<SettingDto> { Field = s => s.Value ?? "Not set", Title = "Value", RowWidth = 20 },
                    new TableField<SettingDto> { Field = s => s.LastUpdated!, Title = "Last updated", RowWidth = 32 });
            }

            return;
        }

        var valueResult = await settingRepository.GetValueAsync(key, type, cancellationToken);

        if (valueResult.HasValue)
        {
            await managedStream.Out.WriteLineAsync($"{key}: {valueResult.Result}", cancellationToken);
            return;
        }

        await managedStream.Error.WriteLineAsync($"Unable to read value: {valueResult.Exception?.Message ?? "Unknown issue"}", cancellationToken);
    }
}
