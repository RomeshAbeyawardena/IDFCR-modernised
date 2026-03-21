using IDFCR.Abstractions.Mapper;

namespace IDFCR.Abstractions.Persistence.Tests
{
    public class Customer : MapperBase<ICustomer>, ICustomer
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedTimestampUtc { get; set; }
        public DateTimeOffset? ModifiedTimestampUtc { get; set; }

        public override void Map(ICustomer source)
        {
            Id = source.Id;
            CreatedTimestampUtc = source.CreatedTimestampUtc;
            ModifiedTimestampUtc = source.ModifiedTimestampUtc;
        }
    }
}
