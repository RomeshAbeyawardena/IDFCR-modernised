using System;
using System.Collections.Generic;
using System.Text;
using IDFCR.Abstractions.Cli.Operations;

namespace BuildTools.Cli.Features.Packages.Tags;

[FeatureCommand(PackageRootOperation.Prefix, CommandName)]
public class PackageTagRootOperation(IServiceProvider serviceProvider)
    : InjectableCommandOperationBase<PackageTagRootOperation>(serviceProvider, PackageRootOperation.Prefix,
        CommandName, typeof(PackageRootOperation))
{
    public const string Prefix = $"{PackageRootOperation.Prefix}-tag";
    public const string CommandName = "tag";
}
