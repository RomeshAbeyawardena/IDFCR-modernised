using BuildTools.Infrastructure.SqlServer.Features.Tags;

namespace BuildTools.Infrastructure.SqlServer.Features.Packages;

public class PackageTagEntity
{
    public Guid Id { get; set; }
    public Guid PackageId { get; set; }
    public Guid TagId { get; set; }

    public virtual PackageEntity? Package { get; set; }
    public virtual TagEntity? Tag { get; set; }
}
