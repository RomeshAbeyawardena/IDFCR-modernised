using BuildTools.Infrastructure.Features.Packages;
using BuildTools.Infrastructure.Features.Packages.Version;
using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using IDFCR.Abstractions.Results.Extensions;

namespace BuildTools.Cli.Features.Packages.Version;

//increments package version by adding a new version with a new revision number, and making it the latest version. If the package doesn't exist, it will be created with version 1.0.0
[FeatureCommand(PackageVersionRootOperation.Prefix, CommandName)]
public class PackageVersionIncrementOperation(IServiceProvider serviceProvider, IManagedStream managedStream, IPackageRepository packageRepository, IPackageVersionRepository packageVersionRepository, TimeProvider timeProvider)
    : InjectableCommandOperationBase<PackageVersionIncrementOperation>(serviceProvider, PackageVersionRootOperation.Prefix,
        CommandName, typeof(PackageVersionRootOperation))
{
    public const string CommandName = "add";

    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var (hasNamespace, packageNamespace) = (await this.GetRequiredField(managedStream, command, 0, "Package namespace", cancellationToken, false, "namespace")).AsValueOrDefault(out var isParameter);
        var (hasVersionPrefix, packageVersionPrefix) = (await this.GetRequiredField(managedStream, command, 1, "Package version prefix", cancellationToken, isParameter, "version-prefix")).AsValueOrDefault(out isParameter);
        var packageName = await this.GetOptionalField(managedStream, command, cancellationToken, isParameter, "package-name");
        var commitId = await this.GetOptionalField(managedStream, command, cancellationToken, isParameter, "commit-id");

        var parameters = Parameters!;

        var isExternalTool = parameters.TryGetValue("external-tool", out var parameter) && parameter.IsFlag;

        if ((!hasNamespace || string.IsNullOrWhiteSpace(packageName))
            && !hasVersionPrefix)
        {
            await managedStream.Error.WriteLineAsync("Validation failed: A package namespace or name must be specified with a version prefix", cancellationToken);
            return;
        }

        if (string.IsNullOrWhiteSpace(packageVersionPrefix) || packageVersionPrefix.Count(c => c == '.') < 2)
        {
            await managedStream.Error.WriteLineAsync("Validation failed: Version prefix does not meet requirements, e.g. `1.0.0`", cancellationToken);
            return;
        }

        var packageResult = await packageRepository.GetPackageAsync(packageName, packageNamespace, cancellationToken);

        if (!packageResult.HasValue)
        {
            await managedStream.Error.WriteLineAsync($"Unable to find package: {packageResult.Exception?.Message ?? "Unknown error"}", cancellationToken);
            return;
        }


        var latestPackageResult = await packageVersionRepository.GetLatestVersionAsync(packageResult.Result.Id, cancellationToken);

        var newRevisionNumber = 0;

        if (latestPackageResult.HasValue)
        {
            newRevisionNumber = latestPackageResult.Result.RevisionNumber + 1;
        }

        var upsertResult = await packageVersionRepository.UpsertAsync(new Shared.Features.Packages.Version.PackageVersion
        {
            VersionPrefix = packageVersionPrefix,
            RevisionNumber = newRevisionNumber,
            ReleaseDateTimestampUtc = timeProvider.GetUtcNow(),
            PackageId = packageResult.Result.Id!,
            CommitId =  commitId ?? string.Empty
        }, cancellationToken);

        if (!upsertResult.IsSuccess)
        {
            await managedStream.Error.WriteLineAsync($"Unable to save package version: {upsertResult.Exception?.Message ?? "Unknown error"}", cancellationToken);
            return;
        }

        await packageVersionRepository.SaveChangesAsync(cancellationToken);

        if (isExternalTool)
        {
            await managedStream.Out.WriteLineAsync($"{packageVersionPrefix}.{newRevisionNumber}", cancellationToken);
            return;
        }

        await managedStream.Out.WriteLineAsync($"Added: Package version: {packageNamespace}, Package name: {packageName}, {packageVersionPrefix}.{newRevisionNumber}, {packageName}", cancellationToken);

    }
}
