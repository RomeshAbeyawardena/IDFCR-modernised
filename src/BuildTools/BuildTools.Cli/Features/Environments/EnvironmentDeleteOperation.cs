using IDFCR.Abstractions.Cli.Operations;
using BuildTools.Infrastructure.Features.Environments;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Results.Extensions;

namespace BuildTools.Cli.Features.Environments;

[FeatureCommand(EnvironmentRootOperation.Prefix, CommandName)]
public class EnvironmentDeleteOperation(IServiceProvider serviceProvider, IManagedStream managedStream,
    IEnvironmentRepository environmentRepository)
    : InjectableCommandOperationBase<EnvironmentDeleteOperation>(serviceProvider, EnvironmentRootOperation.Prefix,
        CommandName, typeof(EnvironmentRootOperation))
{
    public const string CommandName = "delete";

    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var (hasExternalReference, externalReference) = (await this.GetRequiredField(managedStream, command, 0, "Environment external reference", cancellationToken, false, "external-reference")).AsValueOrDefault(out _);

        var isValid = hasExternalReference;

        if (!isValid)
        {
            //todo: display help
            return;
        }

        var foundEntry = (await environmentRepository.GetEnvironmentAsync(new GetPagedEnvironmentQuery
        {
            ExternalReference = externalReference
        }, cancellationToken)).GetResultOrDefault();

        if (foundEntry is null)
        {
            await managedStream.Error.WriteLineAsync($"Unable to delete environment: Environment with external reference '{externalReference}' not found.", cancellationToken);
            return;
        }

        if (foundEntry.Id is not Guid id)
        {
            await managedStream.Error.WriteLineAsync("Unexpected condition: Entity is not in the correct state to be removed, operation aborted!", cancellationToken);
            return;
        }

        var isInUse = await environmentRepository.IsEnvironmentInUseAsync(id, cancellationToken);
        if (isInUse)
        {
            await managedStream.Error.WriteLineAsync($"Unable to delete environment: Environment '{externalReference} ({foundEntry.Name})' is currently in use by one or more settings.", cancellationToken);
            return;
        }

        var shouldForce = Parameters?.TryGetValue("force", out var isForce) == true && isForce.IsFlag;

        if (!shouldForce)
        {
            await managedStream.Error.WriteLineAsync("⚠️  This action cannot be undone. This will permanently delete the environment.", cancellationToken);
            await managedStream.Error.WriteLineAsync($"Environment: {externalReference} ({foundEntry.Name})", cancellationToken);
            await managedStream.Error.WriteLineAsync(string.Empty, cancellationToken);

            var confirmation = await managedStream.In.ReadLineAsync(cancellationToken);

            if (confirmation?.Trim() != externalReference)
            {
                await managedStream.Out.WriteLineAsync("Confirmation failed. Deletion cancelled.", cancellationToken);
                return;
            }
        }

        var result = await environmentRepository.DeleteAsync(id, cancellationToken);
        if (result.IsSuccess)
        {
            await environmentRepository.SaveChangesAsync(cancellationToken);
            await managedStream.Out.WriteLineAsync($"Environment '{externalReference} ({foundEntry.Name})' successfully deleted.", cancellationToken);
            return;
        }

        await managedStream.Error.WriteLineAsync($"Unable to delete environment: {result.Exception?.Message ?? "Unknown error"}", cancellationToken);
    }
}