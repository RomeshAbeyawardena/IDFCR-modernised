using IDFCR.Abstractions.Mapper;

namespace IDFCR.Abstractions.Persistence.Tests
{
    public class DbCustomer : MapperBase<ICustomer>, ICustomer
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedTimestampUtc { get; set; }
        public DateTimeOffset? ModifiedTimestampUtc { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;

        public override void Map(ICustomer source)
        {
            Id = source.Id;
            FirstName = source.FirstName;
            MiddleName = source.MiddleName;
            LastName = source.LastName;
            CreatedTimestampUtc = source.CreatedTimestampUtc;
            ModifiedTimestampUtc = source.ModifiedTimestampUtc;
        }
    }
}
