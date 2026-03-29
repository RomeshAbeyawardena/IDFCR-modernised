using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Cli.Operations;
using BuildTools.Infrastructure.Features.Tags;

namespace BuildTools.Cli.Features.Tags;

[FeatureCommand(TagRootOperation.Prefix, CommandName)]
public class TagReadOperation(IServiceProvider serviceProvider, IManagedStream managedStream, ITagRepository tagRepository)
    : InjectableCommandOperationBase<TagReadOperation>(serviceProvider, TagRootOperation.Prefix, CommandName, typeof(TagRootOperation))
{
    public const string CommandName = "read";
    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var name = await this.GetOptionalField(managedStream, command, cancellationToken, false, "name");
        var outputType = await this.GetOptionalField(managedStream, command, cancellationToken, true, "output-type");

        if (string.IsNullOrWhiteSpace(name))
        {
            var pagedResult = await tagRepository.GetPagedAsync(new GetPagedTagsQuery
            {
                PageSize = 20,
                Name = name
            }, cancellationToken);

            if (pagedResult.HasValue)
            {
                if (outputType == "json")
                {
                    await managedStream.Out.WriteLineAsync(pagedResult.Result.Jsonify(System.Text.Json.JsonSerializerOptions.Web), cancellationToken);
                    return;
                }

                await managedStream.DisplayPagedTable(pagedResult, t => t.Map<TagDto>(t), cancellationToken);
            }
            return;
        }

        var valueResult = await tagRepository.GetTagAsync(name, cancellationToken);

        if (valueResult.HasValue)
        {
            if (outputType == "json")
            {
                await managedStream.Out.WriteLineAsync(valueResult.Result.Jsonify(System.Text.Json.JsonSerializerOptions.Web), cancellationToken);
                return;
            }

            await managedStream.Error.WriteLineAsync("Unsupported format: Please provide a valid output-type", cancellationToken);

            return;
        }

        await managedStream.Error.WriteLineAsync($"Unable to read value: {valueResult.Exception?.Message ?? "Unknown issue"}", cancellationToken);

    }
}