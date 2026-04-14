using BuildTools.Cli.Common;
using BuildTools.Infrastructure.Features.Settings;
using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using IDFCR.Abstractions.Results.Extensions;

namespace BuildTools.Cli.Features.Settings;

[FeatureCommand(SettingRootOperation.Prefix, CommandName)]
public class SettingReadOperation(IServiceProvider serviceProvider, IManagedStream managedStream, ISettingRepository settingRepository)
    : ReadCommandOperationBase<SettingReadOperation>(serviceProvider, managedStream, SettingRootOperation.Prefix, CommandName, typeof(SettingRootOperation))
{
    public const string CommandName = "read";

    private string? _key;
    private string? _type;

    protected override async Task AcquireFieldsAsync(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        _key = await this.GetOptionalField(ManagedStream, command, cancellationToken, false, "key");
        _type = await this.GetOptionalField(ManagedStream, command, cancellationToken, true, "type");
    }

    protected override async Task InvokeReadAsync(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_key))
        {
            var pagedRequest = new GetPagedSettingsQuery
            {
                PageSize = 20,
                Key = _key,
                Type = _type
            };

            pagedRequest.MapQuery(PagedQuery);

            var pagedResult = await settingRepository.GetPagedAsync(pagedRequest, cancellationToken);

            if (pagedResult.HasValue)
                await WritePagedResultAsync(pagedResult, t => t.Map<SettingDto>(), cancellationToken,
                    new TableField<SettingDto> { Field = s => s.Key, Title = "Setting Key", RowWidth = 20 },
                    new TableField<SettingDto> { Field = s => s.Value ?? "Not set", Title = "Value", RowWidth = 20 },
                    new TableField<SettingDto> { Field = s => s.LastUpdated!, Title = "Last updated", RowWidth = 32 });
            return;
        }

        var valueResult = await settingRepository.GetValueAsync(_key, _type, cancellationToken);

        if (valueResult.HasValue)
        {
            if (IsJson)
            {
                (string Key, string Value) settingValue = new(_key, valueResult.Result);
                await WriteJsonAsync(settingValue, cancellationToken);
                return;
            }

            await ManagedStream.Out.WriteLineAsync($"{_key}: {valueResult.Result}", cancellationToken);
            return;
        }

        await ManagedStream.Error.WriteLineAsync($"Unable to read value: {valueResult.Exception?.Message ?? "Unknown issue"}", cancellationToken);
    }
}
