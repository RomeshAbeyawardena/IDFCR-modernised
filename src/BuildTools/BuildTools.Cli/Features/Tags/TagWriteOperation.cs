using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Cli.Operations;
using BuildTools.Infrastructure.Features.Tags;
using BuildTools.Shared.Features.Tags;
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

        var result = await tagRepository.UpsertAsync(new Tag
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
        if(Parameters!.TryGetValue("tags", out var tags) && !string.IsNullOrWhiteSpace(tags.Value))
        {
            //a csv of tags tag1:Tag 1,tag2:Tag 2
            var tagItems = tags.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            List<Tag> tagList = [];
            foreach(var tagItem in tagItems)
            {
                if (!tagItem.Contains(':'))
                {
                    continue;
                }

                var delimited = tagItem.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                var name = delimited[0];
                
                if (delimited.Length == 2)
                {
                    tagList.Add(new Tag { 
                        Name = name, 
                        DisplayName = delimited[1] 
                    });
                }
            }

            var existingTags = (await tagRepository.GetExistingTagsAsync([.. tagList.Select(x => x.Name)], cancellationToken)).GetResultOrDefault();

            var tagsToAdd = tagList.AsEnumerable();

            if (existingTags is not null)
            {
                tagsToAdd = tagList.Where(x => !existingTags.Any(t => t == x));
            }

            var addResult = await tagRepository.AddTagsAsync([.. tagsToAdd], cancellationToken);

            if (addResult.IsSuccess)
            {
                await tagRepository.SaveChangesAsync(cancellationToken);
            }

            return;
        }

        await SimpleUpsert(command, cancellationToken);
    }
}
