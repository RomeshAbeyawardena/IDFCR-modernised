using BuildTools.Infrastructure.Features.Tags;
using BuildTools.Shared.Contracts.Features.Tags;
using BuildTools.Shared.Features.Tags;
using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using IDFCR.Abstractions.Results.Extensions;
using MediatR;

namespace BuildTools.Cli.Features.Tags;

[FeatureCommand(TagRootOperation.Prefix, CommandName)]
public class TagWriteOperation(IServiceProvider serviceProvider, IManagedStream managedStream, IMediator mediator, ITagRepository tagRepository)
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

        var foundEntry = (await mediator.Send(new GetTagQuery { Name = name }, cancellationToken)).GetResultOrDefault();

        var result = await mediator.Send(new UpsertTagCommand
        {
            CommitChanges = true,
            Tag = new Shared.Contracts.Features.Tags.TagDto
            {
                Id = foundEntry?.Id,
                Name = foundEntry?.Name ?? name!,
                DisplayName = displayName
            }
        }, cancellationToken);

        if (result.IsSuccess)
        {
            var verb = foundEntry is null ? "added" : "updated";
            await managedStream.Out.WriteLineAsync($"Tag '{name}' successfully {verb}.", cancellationToken);
            return;
        }

        await managedStream.Error.WriteLineAsync($"Unable to write tag: {result.Exception?.Message ?? "Unknown error"}", cancellationToken);
    }

    internal async Task MultipleUpsert(string tags, CancellationToken cancellationToken)
    {
        //a csv of tags tag1:Tag 1,tag2:Tag 2
        var tagItems = tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        List<Tag> tagList = [];
        foreach (var tagItem in tagItems)
        {
            if (!tagItem.Contains(':'))
            {
                continue;
            }

            var delimited = tagItem.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var name = delimited[0];

            if (delimited.Length == 2)
            {
                tagList.Add(new Tag
                {
                    Name = name,
                    DisplayName = delimited[1]
                });
            }
        }

        tagList = [.. tagList.DistinctBy(x => x.Name)];

        var existingTagsDto = (await mediator.Send(new GetTagsQuery { Names = [.. tagList.Select(x => x.Name)] }, cancellationToken)).GetResultOrDefault();

        var tagsToAdd = tagList.AsEnumerable();

        if (existingTagsDto is not null)
        {
            var existingTags = existingTagsDto.Select(x => x.Map<Tag>());

            tagsToAdd = tagList.Where(x => !existingTags.Any(t => t == x));

            foreach (var tag in existingTags)
            {
                var foundTag = tagList.FirstOrDefault(t => t == tag);

                if (foundTag is null || foundTag.DisplayName == tag.DisplayName)
                {
                    continue;
                }

                tag.DisplayName = foundTag.DisplayName;
                //this will be too expensive to be handled by a pipeline, especially if it has many tags and pathways
                await tagRepository.UpsertAsync(tag, cancellationToken);
            }
        }

        var addResult = await tagRepository.AddTagsAsync([.. tagsToAdd], cancellationToken);


        if (addResult.IsSuccess)
        {
            await tagRepository.SaveChangesAsync(cancellationToken);
        }

        return;
    }

    protected override Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        if (Parameters!.TryGetValue("tags", out var tags) && !string.IsNullOrWhiteSpace(tags.Value))
        {
            return MultipleUpsert(tags.Value, cancellationToken);
        }

        return SimpleUpsert(command, cancellationToken);
    }
}
