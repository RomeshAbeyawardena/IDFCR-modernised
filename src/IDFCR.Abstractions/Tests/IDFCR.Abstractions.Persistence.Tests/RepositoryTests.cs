using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Interceptors.Interceptors;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Persistence.Extensions;
using IDFCR.Abstractions.Results.Exceptions;
using Moq;
using NUnit.Framework;

namespace IDFCR.Abstractions.Persistence.Tests
{
    internal class InternalMemoryMockRepository<TCommon, TDb, T>(IEntityInterceptorFactory entityInterceptorFactory) 
        : RepositoryBase<TCommon, TDb, T, Guid>(entityInterceptorFactory)
        where TDb : class, IMapper<TCommon>, TCommon, IIdentifiable<Guid>
        where T : class, IMapper<TCommon>, TCommon
    {
        private readonly List<TDb> entries = [];
        public bool IsHandledFlag { private get; set; } = false;
        protected override bool IsHandled(Exception exception)
        {
            return IsHandledFlag;
        }

        protected override Task<Guid> OnAddAsync(TDb entry, T rawEntry, CancellationToken cancellationToken)
        {
            //simulate db 
            entry.Id = Guid.NewGuid();
            entries.Add(entry);
            return Task.FromResult(entry.Id);
        }

        protected override Task<bool> OnDeleteAsync(Guid key, CancellationToken cancellationToken)
        {
            var entry = entries.Find(x => x.Id == key);

            if (entry is null)
            {
                return Task.FromResult(false);
            }

            entries.Remove(entry);

            return Task.FromResult(true);
        }

        protected override Task<TDb?> OnFindAsync(Guid key, bool trackChanges, CancellationToken cancellationToken)
        {
            return Task.FromResult(entries.Find(x => x.Id == key));
        }

        protected override Task<TDb?> OnFindAsync(object[] keys, bool trackChanges, CancellationToken cancellationToken)
        {
            if (keys.Length < 1)
            {
                return Task.FromResult<TDb?>(null);
            }
            var key = keys.FirstOrDefault();
            if (key is null || !Guid.TryParse(key?.ToString(), out var id))
            {
                return Task.FromResult<TDb?>(null);
            }

            return OnFindAsync(id, trackChanges, cancellationToken);
        }

        protected override Task<Guid> OnUpdateAsync(TDb entry, T rawEntry, CancellationToken cancellationToken)
        {
            var foundEntry = entries.Find(x => x.Id == entry.Id) ?? throw new EntityNotFoundException(typeof(TDb), entry.Id);
            foundEntry.Apply(entry);
            return Task.FromResult(foundEntry.Id);
        }
    }

    public interface ICustomer : IMapper<ICustomer>, IIdentifiable<Guid>, IAuditCreatedTimestamp, IAuditModifiedTimestamp
    {

    }

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

    public class DbCustomer : MapperBase<ICustomer>, ICustomer
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

    [TestFixture]
    public class RepositoryTests
    {
        private Mock<IEntityInterceptorFactory> _factoryMock;
        private InternalMemoryMockRepository<ICustomer, DbCustomer, Customer> _mockRepository;
        [SetUp]
        public void SetUp() 
        {
            _factoryMock = new();
            _mockRepository = new(_factoryMock.Object);
        }

        [Test]
        public async Task AddAsync_Should_SetCreatedTimestamp_And_NotModified()
        {
            var timeProvider = new FakeTimeProvider(
                new DateTimeOffset(2025, 03, 1, 10, 40, 0, TimeSpan.Zero));

            var interceptors = new List<IEntityInterceptor>
    {
        new AuditCreatedTimestampEntityInterceptor(timeProvider),
        new AuditModifiedTimestampEntityInterceptor(timeProvider)
    };

            var factory = new DefaultEntityInterceptorFactory(interceptors);

            var repo = new InternalMemoryMockRepository<ICustomer, DbCustomer, Customer>(factory);

            var entity = new Customer();

            var id = await repo.UpsertAsync(entity, CancellationToken.None);

            var stored = await repo.FindAsync(id, trackChanges: false, CancellationToken.None);

            Assert.That(stored, Is.Not.Null);
            Assert.That(stored!.CreatedTimestampUtc,
                Is.EqualTo(timeProvider.GetUtcNow()));
            Assert.That(stored.ModifiedTimestampUtc, Is.Null);
        }
    }
}
