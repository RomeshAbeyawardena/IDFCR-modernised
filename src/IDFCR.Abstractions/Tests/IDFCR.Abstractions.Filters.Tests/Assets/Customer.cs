using IDFCR.Abstractions.Mapper;

namespace IDFCR.Abstractions.Filters.Tests.Assets;

public class Customer : MapperBase<ICustomer>, ICustomer
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = null!;
    public DateTimeOffset CreatedTimestampUtc { get; set; }
    public DateTimeOffset? ModifiedTimestampUtc { get; set; }
    public bool Suppressed { get; set; }

    protected override void MapMembers(ICustomer source)
    {
        Id = source.Id;
        FirstName = source.FirstName;
        MiddleName = source.MiddleName;
        LastName = source.LastName;
        CreatedTimestampUtc = source.CreatedTimestampUtc;
        ModifiedTimestampUtc = source.ModifiedTimestampUtc;
        Suppressed = source.Suppressed;
    }
}
