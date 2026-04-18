using BuildTools.Infrastructure.Features.Environments;
using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using IDFCR.Abstractions.Results.Extensions;

namespace BuildTools.Cli.Features.Environments;

[FeatureCommand(EnvironmentRootOperation.Prefix, CommandName)]
public class EnvironmentWriteOperation(IServiceProvider serviceProvider, IManagedStream managedStream,
    IEnvironmentRepository environmentRepository)
    : InjectableCommandOperationBase<EnvironmentWriteOperation>(serviceProvider, EnvironmentRootOperation.Prefix,
        CommandName, typeof(EnvironmentRootOperation))
{
    public const string CommandName = "write";

    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var (hasExternalReference, externalReference) = (await this.GetRequiredField(managedStream, command, 0, "Environment external reference", cancellationToken, false, "external-reference")).AsValueOrDefault(out var isParameter);
        var (hasName, name) = (await this.GetRequiredField(managedStream, command, 1, "Environment name", cancellationToken, isParameter, "name")).AsValueOrDefault(out isParameter);
        var displayName = await this.GetOptionalField(managedStream, command, cancellationToken, isParameter, "display-name");

        var isValid = hasName && hasExternalReference;

        if (!isValid)
        {
            //todo: display help
            return;
        }

        var foundEntry = (await environmentRepository.GetEnvironmentAsync(new GetPagedEnvironmentQuery
        {
            Name = name,
            ExternalReference = externalReference
        }, cancellationToken)).GetResultOrDefault();

        //ensure another environment with the same external reference does not already exist
        var foundEntryWithExternalReference = (await environmentRepository.GetEnvironmentAsync(new GetPagedEnvironmentQuery
        {
            ExternalReference = externalReference
        }, cancellationToken)).GetResultOrDefault();

        if (foundEntry is null && foundEntryWithExternalReference is not null)
        {
            await managedStream.Error.WriteLineAsync($"Unable to write environment: An external reference '{externalReference}' already exists.", cancellationToken);
            return;
        }

        var result = await environmentRepository.UpsertAsync(new Shared.Features.Environments.Environment
        {
            Id = foundEntry?.Id,
            DisplayName = displayName ?? foundEntry?.DisplayName,
            Name = foundEntry?.Name ?? name!,
            ExternalReference = foundEntry?.ExternalReference ?? externalReference!
        }, cancellationToken);

        if (result.IsSuccess)
        {
            await environmentRepository.SaveChangesAsync(cancellationToken);
            var verb = foundEntry is null ? "added" : "updated";
            await managedStream.Out.WriteLineAsync($"Environment '{externalReference} ({name})' successfully {verb}.", cancellationToken);
            return;
        }

        await managedStream.Error.WriteLineAsync($"Unable to write environment: {result.Exception?.Message ?? "Unknown error"}", cancellationToken);
    }
}