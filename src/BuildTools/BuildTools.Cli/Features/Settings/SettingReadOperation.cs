using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Cli.Operations;
using BuildTools.Infrastructure.Features.Settings;

namespace BuildTools.Cli.Features.Settings;

[FeatureCommand(SettingRootOperation.Prefix, CommandName)]
public class SettingReadOperation(IServiceProvider serviceProvider, IManagedStream managedStream, ISettingRepository settingRepository) 
    : InjectableCommandOperationBase<SettingReadOperation>(serviceProvider, SettingRootOperation.Prefix, CommandName, typeof(SettingRootOperation))
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
                if (outputType == "json")
                {
                    await managedStream.Out.WriteLineAsync(pagedResult.Result.Jsonify(System.Text.Json.JsonSerializerOptions.Web), cancellationToken);
                    return;
                }

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
            if (outputType == "json")
            {
                (string Key, string Value) value = new(key, valueResult.Result);

                await managedStream.Out.WriteLineAsync(value.Jsonify(System.Text.Json.JsonSerializerOptions.Web), cancellationToken);
                return;
            }
            await managedStream.Out.WriteLineAsync($"{key}: {valueResult.Result}", cancellationToken);
            return;
        }

        await managedStream.Error.WriteLineAsync($"Unable to read value: {valueResult.Exception?.Message ?? "Unknown issue"}", cancellationToken);
    }
}
