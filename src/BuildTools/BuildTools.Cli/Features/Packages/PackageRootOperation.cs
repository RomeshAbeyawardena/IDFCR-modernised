using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Cli.Operations;
using BuildTools.Shared.Features.Packages;
using IDFCR.Abstractions.Persistence;
using IDFCR.Abstractions.Results;

namespace BuildTools.Cli.Features.Packages;

public class PackageRootOperation(IServiceProvider serviceProvider)
    : InjectableCommandOperationRootBase<PackageRootOperation>(serviceProvider, Prefix, CommandName, null)
{
    public const string Prefix = "package";
    public const string CommandName = "package";
}

public interface IPackageRepository : IRepository<Package, Guid>
{
    Task<IUnitResult<Package>> GetPackageAsync(string? name, string? @namespace, CancellationToken cancellationToken);
}

[FeatureCommand(PackageRootOperation.Prefix,CommandName)]
public class PackageReadOperation(IServiceProvider serviceProvider, IManagedStream managedStream, 
    IPackageRepository packageRepository)
    : InjectableCommandOperationBase<PackageReadOperation>(serviceProvider, PackageRootOperation.Prefix,
        CommandName, typeof(PackageRootOperation))
{
    public const string CommandName = "read";
    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var @namespace = await this.GetOptionalField(managedStream, command, cancellationToken, false, "namespace");
        var name = await this.GetOptionalField(managedStream, command, cancellationToken, true, "name");
        var outputType = await this.GetOptionalField(managedStream, command, cancellationToken, true, "output-type");

        if (string.IsNullOrWhiteSpace(@namespace) || string.IsNullOrEmpty(name))
        {
            var pagedResult = await packageRepository.GetPagedAsync(new GetPagedPackagesQuery
            {
                PageSize = 20,
                Name = name,
                Namespace = @namespace
            }, cancellationToken);

            if (pagedResult.HasValue)
            {
                await managedStream.DisplayPagedTable(pagedResult, t => t.Map<PackageDto>(t), cancellationToken);
            }
        }

        var valueResult = await packageRepository.GetPackageAsync(name, @namespace, cancellationToken);

        if(valueResult.HasValue)
        {
            if (outputType == "Powershell")
            {
                return;
            }
        }

        await managedStream.Error.WriteLineAsync($"Unable to read package: {valueResult.Exception?.Message ?? "Unknown issue"}", cancellationToken);
    }
}