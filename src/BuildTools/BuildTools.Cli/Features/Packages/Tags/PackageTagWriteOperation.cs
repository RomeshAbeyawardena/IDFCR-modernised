using BuildTools.Infrastructure.Features.Packages;
using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using IDFCR.Abstractions.Results;

namespace BuildTools.Cli.Features.Packages.Tags;

[FeatureCommand(PackageTagRootOperation.Prefix, CommandName)]
public class PackageTagWriteOperation(IServiceProvider serviceProvider, IManagedStream managedStream, IPackageRepository packageRepository)
    : InjectableCommandOperationBase<PackageTagWriteOperation>(serviceProvider, PackageTagRootOperation.Prefix,
        CommandName, typeof(PackageTagRootOperation))
{
    public const string CommandName = "write";

    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var (hasNamespace, packageNamespace) = (await this.GetRequiredField(managedStream, command, 0, "Package namespace", cancellationToken, false, "namespace")).AsValueOrDefault(out var isParameter);
        var (hasTags, rawTags) = (await this.GetRequiredField(managedStream, command, 1, "Tags (CSV, e.g. tag1,tag2)", cancellationToken, isParameter, "tags")).AsValueOrDefault(out _);

        if (!hasNamespace || !hasTags)
        {
            return;
        }

        var tagList = rawTags!.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var parameters = Parameters!;
        var isUnassign = parameters.TryGetValue("unassign", out var unassignParam) && unassignParam.IsFlag;
        var isJson = parameters.TryGetValue("json", out var jsonParam) && jsonParam.IsFlag;

        var request = new TagAssignmentRequest(PackageNamespace: packageNamespace, Tags: tagList);

        if (isUnassign)
        {
            var result = await packageRepository.TryUnassignTags(request, cancellationToken);

            if (!result.IsSuccess)
            {
                await managedStream.Error.WriteLineAsync($"Unable to unassign tags: {result.Exception?.Message ?? "Unknown error"}", cancellationToken);
                return;
            }

            await packageRepository.SaveChangesAsync(cancellationToken);

            if (isJson)
            {
                await managedStream.Out.WriteLineAsync(result.Result.Jsonify(System.Text.Json.JsonSerializerOptions.Web), cancellationToken);
                return;
            }

            IUnitPagedResult<TagUnassignmentResult>? unitPagedResult = null;
            if (result.HasValue)
            {
                unitPagedResult = UnitPagedResult.FromResult(result.Result, result.Result.Count(), new PagedQuery());
            }

            if (unitPagedResult is null)
            {
                await managedStream.Error.WriteLineAsync($"Unable to display results: {result.Exception?.Message ?? "Unknown Error"}", cancellationToken);
                return;
            }

            await managedStream.DisplayPagedTable(unitPagedResult, t => t, cancellationToken,
                new TableField<TagUnassignmentResult> { Field = t => t.TagName, Title = "Tag", RowWidth = 20 },
                new TableField<TagUnassignmentResult> { Field = t => t.AssignmentStatus, Title = "Status", RowWidth = 15 }
            );
        }
        else
        {
            var result = await packageRepository.TryAssignTags(request, cancellationToken);

            if (!result.IsSuccess)
            {
                await managedStream.Error.WriteLineAsync($"Unable to assign tags: {result.Exception?.Message ?? "Unknown error"}", cancellationToken);
                return;
            }

            await packageRepository.SaveChangesAsync(cancellationToken);

            if (isJson)
            {
                await managedStream.Out.WriteLineAsync(result.Result.Jsonify(System.Text.Json.JsonSerializerOptions.Web), cancellationToken);
                return;
            }

            IUnitPagedResult<TagAssignmentResult>? unitPagedResult = null;
            if (result.HasValue)
            {
                unitPagedResult = UnitPagedResult.FromResult(result.Result, result.Result.Count(), new PagedQuery());
            }

            if (unitPagedResult is null)
            {
                await managedStream.Error.WriteLineAsync($"Unable to display results: {result.Exception?.Message ?? "Unknown Error"}", cancellationToken);
                return;
            }

            await managedStream.DisplayPagedTable(unitPagedResult, t => t, cancellationToken,
                new TableField<TagAssignmentResult> { Field = t => t.TagName, Title = "Tag", RowWidth = 20 },
                new TableField<TagAssignmentResult> { Field = t => t.AssignmentStatus, Title = "Status", RowWidth = 15 }
            );
        }
    }
}