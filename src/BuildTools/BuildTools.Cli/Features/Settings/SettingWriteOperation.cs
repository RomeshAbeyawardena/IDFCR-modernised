using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Cli.Operations;
using BuildTools.Infrastructure.Features.Settings;
using IDFCR.Abstractions.Results.Extensions;

namespace BuildTools.Cli.Features.Settings;

[FeatureCommand(SettingRootOperation.Prefix, CommandName)]
public class SettingWriteOperation(IServiceProvider serviceProvider, IManagedStream managedStream, ISettingRepository settingRepository)
    : InjectableCommandOperationBase<SettingWriteOperation>(serviceProvider, SettingRootOperation.Prefix, CommandName, typeof(SettingRootOperation))
{
    public const string CommandName = "write";

    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var (hasKeyValue, key) = (await this.GetRequiredField(managedStream, command!, 0, "Setting key", cancellationToken, false, "key")).AsValueOrDefault(out var isParameter);
        var (hasTypeValue, type) = (await this.GetRequiredField(managedStream, command!, 1, "Setting type", cancellationToken, isParameter, "type")).AsValueOrDefault(out isParameter);
        var (hasValue, value) = (await this.GetRequiredField(managedStream, command!, 2, "Setting value", cancellationToken, isParameter, "value")).AsValueOrDefault();

        bool isValid = hasKeyValue && hasTypeValue && hasValue;

        if (!isValid)
        {
            //TODO: Display help
            return;
        }

        var foundEntry = (await settingRepository.GetSettingAsync(key!, type, cancellationToken)).GetResultOrDefault();

        if (foundEntry is not null && StringComparer.OrdinalIgnoreCase.Equals(foundEntry.Value, value))
        {
            await managedStream.Error.WriteLineAsync("No changes detected", cancellationToken);
            return;
        }

        var result = await settingRepository.UpsertAsync(new Shared.Features.Settings.Setting
        {
            Id = foundEntry?.Id,
            Key = foundEntry?.Key ?? key!,
            Type = foundEntry?.Type ?? type!,
            Value = value
        }, cancellationToken);

        if (result.IsSuccess)
        {
            await settingRepository.SaveChangesAsync(cancellationToken);
            var verb = foundEntry is null ? "added" : "updated";
            await managedStream.Out.WriteLineAsync($"System setting '{key}' successfully {verb}.", cancellationToken);
            return;
        }

        await managedStream.Error.WriteLineAsync($"Unable to write setting: {result.Exception?.Message ?? "Unknown error"}", cancellationToken);

    }
}