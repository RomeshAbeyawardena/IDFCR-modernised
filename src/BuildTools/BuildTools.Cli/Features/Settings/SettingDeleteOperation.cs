using BuildTools.Infrastructure.Features.Settings;
using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using IDFCR.Abstractions.Results.Extensions;

namespace BuildTools.Cli.Features.Settings;

[FeatureCommand(SettingRootOperation.Prefix, CommandName)]
public class SettingDeleteOperation(IServiceProvider serviceProvider, IManagedStream managedStream, ISettingRepository settingRepository)
    : InjectableCommandOperationBase<SettingDeleteOperation>(serviceProvider, SettingRootOperation.Prefix, CommandName, typeof(SettingRootOperation))
{
    public const string CommandName = "delete";

    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var (hasKey, key) = (await this.GetRequiredField(managedStream, command, 0, "Setting key", cancellationToken, false, "key"))
            .AsValueOrDefault(out var isParameter);
        var (hasType, type) = (await this.GetRequiredField(managedStream, command, 1, "Setting type", cancellationToken, isParameter, "type"))
            .AsValueOrDefault(out _);

        if (!hasKey || !hasType || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(type))
        {
            //todo: display help
            return;
        }

        var foundEntry = (await settingRepository.GetSettingAsync(key, type, cancellationToken)).GetResultOrDefault();

        if (foundEntry is null)
        {
            await managedStream.Error.WriteLineAsync($"Unable to delete setting: Setting '{key}' of type '{type}' not found.", cancellationToken);
            return;
        }

        if (foundEntry.Id is not Guid id)
        {
            await managedStream.Error.WriteLineAsync("Unexpected condition: Entity is not in the correct state to be removed, operation aborted!", cancellationToken);
            return;
        }

        var shouldForce = Parameters?.TryGetValue("force", out var isForce) == true && isForce.IsFlag;

        if (!shouldForce)
        {
            var confirmationToken = $"{foundEntry.Key}:{foundEntry.Type}";

            await managedStream.Error.WriteLineAsync("⚠️  This action cannot be undone. This will permanently delete the setting.", cancellationToken);
            await managedStream.Error.WriteLineAsync($"Setting: {foundEntry.Key} ({foundEntry.Type})", cancellationToken);
            await managedStream.Error.WriteLineAsync($"Type '{confirmationToken}' to confirm.", cancellationToken);
            await managedStream.Error.WriteLineAsync(string.Empty, cancellationToken);

            var confirmation = await managedStream.In.ReadLineAsync(cancellationToken);

            if (!StringComparer.Ordinal.Equals(confirmation?.Trim(), confirmationToken))
            {
                await managedStream.Out.WriteLineAsync("Confirmation failed. Deletion cancelled.", cancellationToken);
                return;
            }
        }

        var result = await settingRepository.DeleteAsync(id, cancellationToken);
        if (result.IsSuccess)
        {
            await settingRepository.SaveChangesAsync(cancellationToken);
            await managedStream.Out.WriteLineAsync($"System setting '{foundEntry.Key}' ({foundEntry.Type}) successfully deleted.", cancellationToken);
            return;
        }

        await managedStream.Error.WriteLineAsync($"Unable to delete setting: {result.Exception?.Message ?? "Unknown error"}", cancellationToken);
    }
}