using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;

namespace IDFCR.Abstractions.Persistence.Tests.Assets
{
    public class DbCustomer : MapperBase<ICustomer>, ICustomer, IAuditable
    {
        string IAuditable.AuditEntityName { get; } = "Customer";
        public Guid Id { get; set; }
        public DateTimeOffset CreatedTimestampUtc { get; set; }
        public DateTimeOffset? ModifiedTimestampUtc { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;
        public bool Suppressed { get; set; }

        public override void Map(ICustomer source)
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
}
