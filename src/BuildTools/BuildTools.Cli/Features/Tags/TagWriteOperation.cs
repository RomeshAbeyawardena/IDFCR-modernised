using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Cli.Operations;
using BuildTools.Infrastructure.Features.Tags;
using IDFCR.Abstractions.Results.Extensions;

namespace BuildTools.Cli.Features.Tags;

[FeatureCommand(TagRootOperation.Prefix, CommandName)]
public class TagWriteOperation(IServiceProvider serviceProvider, IManagedStream managedStream, ITagRepository tagRepository)
    : InjectableCommandOperationBase<TagWriteOperation>(serviceProvider, TagRootOperation.Prefix, CommandName, typeof(TagRootOperation))
{
    public const string CommandName = "write";

    private async Task SimpleUpsert(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var (hasName, name) = (await this.GetRequiredField(managedStream, command, 0, "Tag name", cancellationToken, false, "name"))
            .AsValueOrDefault(out var isParameter);
        var displayName = await this.GetOptionalField(managedStream, command, cancellationToken, isParameter, "display-name", "friendly-name");

        if (!hasName)
        {
            //todo: display help
            return;
        }

        var foundEntry = (await tagRepository.GetTagAsync(name!, cancellationToken)).GetResultOrDefault();

        var result = await tagRepository.UpsertAsync(new Shared.Features.Tags.Tag
        {
            Id = foundEntry?.Id,
            Name = foundEntry?.Name ?? name!,
            DisplayName = displayName
        }, cancellationToken);

        if (result.IsSuccess)
        {
            await tagRepository.SaveChangesAsync(cancellationToken);
            var verb = foundEntry is null ? "added" : "updated";
            await managedStream.Out.WriteLineAsync($"Tag '{name}' successfully {verb}.", cancellationToken);
            return;
        }

        await managedStream.Error.WriteLineAsync($"Unable to write tag: {result.Exception?.Message ?? "Unknown error"}", cancellationToken);
    }

    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        if(Parameters!.TryGetValue("mult-mode", out var multiMode))
        {

            return;
        }

        await SimpleUpsert(command, cancellationToken);
    }
}
