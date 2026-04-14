using BuildTools.Cli.Common;
using BuildTools.Infrastructure.Features.Packages;
using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using IDFCR.Abstractions.Results.Extensions;

namespace BuildTools.Cli.Features.Packages;

[FeatureCommand(PackageRootOperation.Prefix, CommandName)]
public class PackageReadOperation(IServiceProvider serviceProvider, IManagedStream managedStream,
    IPackageRepository packageRepository)
    : ReadCommandOperationBase<PackageReadOperation>(serviceProvider, managedStream, PackageRootOperation.Prefix,
        CommandName, typeof(PackageRootOperation))
{
    public const string CommandName = "read";

    private string? _namespace;
    private string? _name;

    protected override async Task AcquireFieldsAsync(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        _namespace = await this.GetOptionalField(ManagedStream, command, cancellationToken, false, "namespace");
        _name = await this.GetOptionalField(ManagedStream, command, cancellationToken, true, "name");
    }

    protected override async Task InvokeReadAsync(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_namespace) || string.IsNullOrEmpty(_name))
        {
            var pagedRequest = new GetPagedPackagesQuery
            {
                Name = _name,
                Namespace = _namespace
            };

            pagedRequest.MapQuery(PagedQuery);

            var pagedResult = await packageRepository.GetPagedAsync(pagedRequest, cancellationToken);

            if (pagedResult.HasValue)
                await WritePagedResultAsync(pagedResult, t => t.Map<PackageDto>(), cancellationToken,
                    new TableField<PackageDto> { Field = s => s.Namespace, Title = "Namespace", RowWidth = 20 },
                    new TableField<PackageDto> { Field = s => s.Name, Title = "Name", RowWidth = 40 },
                    new TableField<PackageDto> { Field = s => s.Description, Title = "Description", RowWidth = 80 });
            return;
        }

        var valueResult = await packageRepository.GetPackageAsync(_name, _namespace, cancellationToken);

        if (valueResult.HasValue)
        {
            if (IsJson) { await WriteJsonAsync(valueResult.Result, cancellationToken); return; }
            await ManagedStream.Error.WriteLineAsync("Unsupported format:  Please provide a valid output-type", cancellationToken);
            return;
        }

        await ManagedStream.Error.WriteLineAsync($"Unable to read package: {valueResult.Exception?.Message ?? "Unknown issue"}", cancellationToken);
    }
}