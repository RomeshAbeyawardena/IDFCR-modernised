using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;

namespace IDFCR.Abstractions.Persistence.Tests
{
    public interface ICustomer : IMapper<ICustomer>, IIdentifiable<Guid>, IAuditCreatedTimestamp, IAuditModifiedTimestamp, ISuppressable
    {
        string FirstName { get; }
        string? MiddleName { get; }
        string LastName { get; }

    }
}
