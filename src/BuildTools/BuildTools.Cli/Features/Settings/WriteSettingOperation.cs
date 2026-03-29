using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Cli.Operations;
using BuildTools.Infrastructure.Features.Settings;
using IDFCR.Abstractions.Results.Extensions;

namespace BuildTools.Cli.Features.Settings;

[FeatureCommand(SettingsRootOperation.Prefix, CommandName)]
public class WriteSettingOperation(IServiceProvider serviceProvider, IManagedStream managedStream, ISettingRepository settingRepository, TimeProvider timeProvider)
    : InjectableCommandOperationBase<WriteSettingOperation>(serviceProvider, SettingsRootOperation.Prefix, CommandName, typeof(SettingsRootOperation))
{
    public const string CommandName = "write";

    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var (hasKeyValue, key) = (await this.GetRequiredField(managedStream, command!, 0, "Setting key", cancellationToken, false, "key")).AsValueOrDefault(out var isParameter);
        var (hasTypeValue, type) = (await this.GetRequiredField(managedStream, command!, 1, "Setting type", cancellationToken, isParameter, "type")).AsValueOrDefault(out isParameter);
        var (hasValue, value) = (await this.GetRequiredField(managedStream, command!, 2, "Setting value", cancellationToken, isParameter, "value")).AsValueOrDefault();

        bool isValid = hasKeyValue && hasTypeValue && hasValue;

        if (isValid)
        {
            var foundEntry = (await settingRepository.GetSettingAsync(key!, type, cancellationToken)).GetResultOrDefault();

            var result = await settingRepository.UpsertAsync(new Shared.Features.Settings.Setting
            {
                Id = foundEntry?.Id,
                Key = key!,
                LastUpdatedTimestampUtc = timeProvider.GetUtcNow().UtcDateTime,
                Type = type!,
                Value = value
            }, cancellationToken);

            if (result.IsSuccess)
            {
                await settingRepository.SaveChangesAsync(cancellationToken);
            }

            await managedStream.Error.WriteLineAsync($"Unable to write setting: {result.Exception?.Message ?? "Unknown error"}", cancellationToken);
        }

    }
}