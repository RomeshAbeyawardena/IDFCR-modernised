using BuildTools.Infrastructure.Features.Packages;
using BuildTools.Infrastructure.Features.Packages.Version;
using BuildTools.Shared.Features.Packages.Version;
using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using IDFCR.Abstractions.Results.Extensions;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Options;

namespace BuildTools.Cli.Features.Packages.Version;

//increments package version by adding a new version with a new revision number, and making it the latest version. If the package doesn't exist, it will be created with version 1.0.0
[FeatureCommand(PackageVersionRootOperation.Prefix, CommandName)]
public class PackageVersionIncrementOperation(IServiceProvider serviceProvider, IManagedStream managedStream, 
    IVersionLockRepository versionLockRepository,
    IPackageRepository packageRepository, 
    IOptions<LockRetryConfiguration> lockRetryOptions,
    IPackageVersionRepository packageVersionRepository,
    TimeProvider timeProvider)
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
        var buildToolReference = await this.GetOptionalField(managedStream, command, cancellationToken, isParameter, "build-ref");
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

        
        const int MaximumAttempts = 60;
        const int Timeout = 1000;
        const int LockTimeoutInMinutes = 5;

        bool isLocked = false;
        int attempts = 0;
        VersionLock? lockStatus = null;

        var result = packageResult.Result;

        var lockRetryOpts = lockRetryOptions.Value;
        do
        {
            var versionLockStatus = await versionLockRepository.GetVersionLockAsync(result.Id!,
                packageVersionPrefix, cancellationToken);

            if (versionLockStatus.HasValue)
            {
                lockStatus = versionLockStatus.Result;

                if (isLocked = lockStatus.IsLocked(timeProvider))
                {
                    using (managedStream.BeginWarning())
                    {
                        await managedStream.Error.WriteLineAsync($"The package version is currently locked until {lockStatus.LockedUntilTimestampUtc} by {lockStatus.Reference}", cancellationToken);
                    }
                    await Task.Delay(lockRetryOpts.RetryTimeoutInMilliseconds.GetValueOrDefault(Timeout), cancellationToken);
                    continue;
                }
                else
                {
                    break;
                }
            }
        }
        while (attempts++ < lockRetryOpts.MaximumAttempts.GetValueOrDefault(MaximumAttempts));

        if (isLocked)
        {
            using (managedStream.BeginError())
            {
                await managedStream.Error.WriteLineAsync(@$"The package version is still locked until {lockStatus?.LockedUntilTimestampUtc} by {lockStatus?.Reference}.
                  Unable to acquire resource aborting after {(Timeout * MaximumAttempts)/1000} seconds", cancellationToken);
            }

            return;
        }
        
        var utcNow = timeProvider.GetUtcNow();
        //prepare to lock the resource;
        var upsertLockResult = await versionLockRepository.SetVersionLockAsync(
            result.Id!,
            packageVersionPrefix, buildToolReference,
            utcNow, utcNow.AddMinutes(lockRetryOpts.LockTimeoutInMinutes.GetValueOrDefault(LockTimeoutInMinutes)),
            cancellationToken: cancellationToken);

        if (upsertLockResult.IsSuccess)
        {
            await versionLockRepository.SaveChangesAsync(cancellationToken);
        }
#if DEBUG
        else
        {
            throw upsertLockResult.Exception!;
        }
#endif
        var latestPackageResult = await packageVersionRepository.GetLatestVersionAsync(packageResult.Result.Id, cancellationToken);

        var newRevisionNumber = 0;

        if (latestPackageResult.HasValue)
        {
            newRevisionNumber = latestPackageResult.Result.RevisionNumber + 1;
        }

        var upsertResult = await packageVersionRepository.UpsertAsync(new PackageVersion
        {
            VersionPrefix = packageVersionPrefix,
            RevisionNumber = newRevisionNumber,
            ReleaseDateTimestampUtc = timeProvider.GetUtcNow(),
            PackageId = result.Id!,
            CommitId =  commitId ?? string.Empty
        }, cancellationToken);

        if (!upsertResult.IsSuccess)
        {
            await managedStream.Error.WriteLineAsync($"Unable to save package version: {upsertResult.Exception?.Message ?? "Unknown error"}", cancellationToken);
            return;
        }

        utcNow = timeProvider.GetUtcNow();
        //prepare to unlock the resource;
        upsertLockResult = await versionLockRepository.SetVersionLockAsync(
            packageResult.Result.Id!,
            packageVersionPrefix, buildToolReference,
            lockReleasedTimestampUtc: utcNow,
            revisionId: newRevisionNumber,
            cancellationToken: cancellationToken);
#if DEBUG
        if (!upsertLockResult.IsSuccess)
        {
            throw upsertLockResult.Exception!;
        }
#endif
        //this will save both tables as the same unit of work
        await packageVersionRepository.SaveChangesAsync(cancellationToken);

        if (isExternalTool)
        {
            await managedStream.Out.WriteLineAsync($"{packageVersionPrefix}.{newRevisionNumber}", cancellationToken);
            return;
        }

        await managedStream.Out.WriteLineAsync($"Added: Package version: {packageNamespace}, Package name: {packageName}, {packageVersionPrefix}.{newRevisionNumber}, {packageName}", cancellationToken);

    }
}
