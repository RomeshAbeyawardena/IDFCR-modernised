using BuildTools.Infrastructure.Features.Packages;
using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using IDFCR.Abstractions.Results.Extensions;

namespace BuildTools.Cli.Features.Packages;

[FeatureCommand(PackageRootOperation.Prefix, CommandName)]
public class PackageWriteOperation(IServiceProvider serviceProvider, IManagedStream managedStream,
    IPackageRepository packageRepository)
    : InjectableCommandOperationBase<PackageWriteOperation>(serviceProvider, PackageRootOperation.Prefix,
        CommandName, typeof(PackageRootOperation))
{
    public const string CommandName = "write";
    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var (hasNamespace, @namespace) = (await this.GetRequiredField(managedStream, command, 0, "Package namespace", cancellationToken, false, "namespace")).AsValueOrDefault(out var isParameter);
        var (hasName, name) = (await this.GetRequiredField(managedStream, command, 1, "Package name", cancellationToken, isParameter, "name")).AsValueOrDefault(out isParameter);
        var description = await this.GetOptionalField(managedStream, command, cancellationToken, isParameter, "description", "package-description");
        var alias = await this.GetOptionalField(managedStream, command, cancellationToken, isParameter, "alias", "package-alias");

        var isValid = hasName && hasNamespace;

        if (!isValid)
        {
            //todo: display help
            return;
        }

        var foundEntry = (await packageRepository.GetPackageAsync(name, @namespace, cancellationToken)).GetResultOrDefault();

        var result = await packageRepository.UpsertAsync(new Shared.Features.Packages.Package
        {
            Id = foundEntry?.Id,
            Alias = alias,
            Description = description,
            Name = foundEntry?.Name ?? name!,
            Namespace = foundEntry?.Namespace ?? @namespace!
        }, cancellationToken);

        if (result.IsSuccess)
        {
            await packageRepository.SaveChangesAsync(cancellationToken);
            var verb = foundEntry is null ? "added" : "updated";
            await managedStream.Out.WriteLineAsync($"Package '({@namespace}) {name}' successfully {verb}.", cancellationToken);
            return;
        }

        await managedStream.Error.WriteLineAsync($"Unable to write package: {result.Exception?.Message ?? "Unknown error"}", cancellationToken);
    }
}
