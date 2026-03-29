using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Cli.Operations;
using BuildTools.Infrastructure.Features.Settings;

namespace BuildTools.Cli.Features.Settings;

[FeatureCommand(SettingsRootOperation.Prefix, CommandName)]
public class GetSettingsOperation(IServiceProvider serviceProvider, IManagedStream managedStream, ISettingRepository settingRepository) 
    : InjectableCommandOperationBase<GetSettingsOperation>(serviceProvider, SettingsRootOperation.Prefix, CommandName, typeof(SettingsRootOperation))
{
    public const string CommandName = "read";
    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var key = await this.GetOptionalField(managedStream, Parameters!, cancellationToken, true, "key");
        var type = await this.GetOptionalField(managedStream, Parameters!, cancellationToken, true, "type");
        
        if (string.IsNullOrWhiteSpace(key))
        {
            //generate paged list
            var pagedResult = await settingRepository.GetPagedAsync(new GetPagedSettingsQuery 
            {
                
            }, cancellationToken);

            if (pagedResult.HasValue)
            {
                await managedStream.DisplayPagedTable(pagedResult, t => t.Map<SettingDto>(t), cancellationToken, 
                    new TableField<SettingDto> { Field = s => s.Key, Title = "Setting Key", RowWidth = 32 },
                    new TableField<SettingDto> { Field = s => s.Value ?? "Not set", Title = "Setting Key", RowWidth = 32 },
                    new TableField<SettingDto> { Field = s => s.LastUpdatedTimestampUtc, Title = "Setting Key", RowWidth = 32, Format = x => x is DateTime date ? date.ToString() : string.Empty }
                );
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