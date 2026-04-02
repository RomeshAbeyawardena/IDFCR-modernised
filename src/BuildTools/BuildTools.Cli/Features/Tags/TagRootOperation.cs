using IDFCR.Abstractions.Cli.Operations;

namespace BuildTools.Cli.Features.Tags;

public class TagRootOperation(IServiceProvider serviceProvider)
    : InjectableCommandOperationRootBase<TagRootOperation>(serviceProvider, Prefix, CommandName, null)
{
    public const string Prefix = "tag";
    public const string CommandName = "tag";
}
