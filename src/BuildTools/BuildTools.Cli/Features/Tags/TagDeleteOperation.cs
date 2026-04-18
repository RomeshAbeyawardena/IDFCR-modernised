using BuildTools.Shared.Contracts.Features.Tags;
using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using IDFCR.Abstractions.Results.Extensions;
using MediatR;

namespace BuildTools.Cli.Features.Tags;

[FeatureCommand(TagRootOperation.Prefix, CommandName)]
public class TagDeleteOperation(IServiceProvider serviceProvider, IManagedStream managedStream, IMediator mediator)
    : InjectableCommandOperationBase<TagDeleteOperation>(serviceProvider, TagRootOperation.Prefix, CommandName, typeof(TagRootOperation))
{
    public const string CommandName = "delete";

    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var (hasName, name) = (await this.GetRequiredField(managedStream, command, 0, "Tag name", cancellationToken, false, "name"))
            .AsValueOrDefault(out _);

        if (!hasName || string.IsNullOrWhiteSpace(name))
        {
            //todo: display help
            return;
        }

        var foundEntry = (await mediator.Send(new GetTagQuery { Name = name }, cancellationToken)).GetResultOrDefault();

        if (foundEntry is null)
        {
            await managedStream.Error.WriteLineAsync($"Unable to delete tag: Tag '{name}' not found.", cancellationToken);
            return;
        }

        if (foundEntry.Id is not Guid id)
        {
            await managedStream.Error.WriteLineAsync("Unexpected condition: Entity is not in the correct state to be removed, operation aborted!", cancellationToken);
            return;
        }

        var shouldForce = Parameters?.TryGetValue("force", out var isForce) == true && isForce.IsFlag;

        if (!shouldForce)
        {
            await managedStream.Error.WriteLineAsync("⚠️  This action cannot be undone. This will permanently delete the tag.", cancellationToken);
            await managedStream.Error.WriteLineAsync($"Tag: {foundEntry.Name} ({foundEntry.DisplayName})", cancellationToken);
            await managedStream.Error.WriteLineAsync(string.Empty, cancellationToken);

            var confirmation = await managedStream.In.ReadLineAsync(cancellationToken);

            if (confirmation?.Trim() != foundEntry.Name)
            {
                await managedStream.Out.WriteLineAsync("Confirmation failed. Deletion cancelled.", cancellationToken);
                return;
            }
        }

        var result = await mediator.Send(new DeleteTagCommand { Name = name, CommitChanges = true }, cancellationToken);
        if (result.IsSuccess)
        {
            await managedStream.Out.WriteLineAsync($"Tag '{foundEntry.Name}' successfully deleted.", cancellationToken);
            return;
        }

        await managedStream.Error.WriteLineAsync($"Unable to delete tag: {result.Exception?.Message ?? "Unknown error"}", cancellationToken);
    }
}