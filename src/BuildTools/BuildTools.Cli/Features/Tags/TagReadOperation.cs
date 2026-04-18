using BuildTools.Cli.Common;
using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using MediatR;

namespace BuildTools.Cli.Features.Tags;

[FeatureCommand(TagRootOperation.Prefix, CommandName)]
public class TagReadOperation(IServiceProvider serviceProvider, IManagedStream managedStream, IMediator mediator)
    : ReadCommandOperationBase<TagReadOperation>(serviceProvider, managedStream, TagRootOperation.Prefix, CommandName, typeof(TagRootOperation))
{
    public const string CommandName = "read";

    private string? _name;

    protected override async Task AcquireFieldsAsync(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        _name = await this.GetOptionalField(ManagedStream, command, cancellationToken, false, "name");
    }

    protected override async Task InvokeReadAsync(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_name))
        {
            var pagedResult = await mediator.Send(new Shared.Contracts.Features.Tags.GetPagedTagsQuery
            {
                PageSize = 20,
                Name = _name
            }, cancellationToken);

            if (pagedResult.HasValue)
                await WritePagedResultAsync(pagedResult, t => t.Map<TagDto>(), cancellationToken,
                    new TableField<TagDto> { Field = t => t.Name, Title = "Tag name", RowWidth = 20 },
                    new TableField<TagDto> { Field = t => t.DisplayName, Title = "Friendly name", RowWidth = 20 });
            return;
        }

        var valueResult = await mediator.Send(new Shared.Contracts.Features.Tags.GetTagQuery { Name = _name }, cancellationToken);

        if (valueResult.HasValue)
        {
            if (IsJson) { await WriteJsonAsync(valueResult.Result, cancellationToken); return; }
            await ManagedStream.Error.WriteLineAsync("Unsupported format: Please provide a valid output-type", cancellationToken);
            return;
        }

        await ManagedStream.Error.WriteLineAsync($"Unable to read value: {valueResult.Exception?.Message ?? "Unknown issue"}", cancellationToken);
    }
}