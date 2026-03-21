using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Interceptors.Interceptors;
using IDFCR.Abstractions.Results.Extensions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using NUnit.Framework;

namespace IDFCR.Abstractions.Persistence.Tests
{

    [TestFixture]
    public class RepositoryTests
    {

        private Mock<IEntityInterceptorFactory> _factoryMock;
        private InternalMemoryMockRepository<ICustomer, DbCustomer, Customer> _mockRepository;
        [SetUp]
        public void SetUp()
        {
            _factoryMock = new();
            var timeProvider = new FakeTimeProvider(
                new DateTimeOffset(2025, 03, 1, 10, 40, 0, TimeSpan.Zero));

            IEnumerable<IEntityInterceptor> interceptors = [
                new AuditCreatedTimestampEntityInterceptor(timeProvider),
                new AuditModifiedTimestampEntityInterceptor(timeProvider)
            ];

            _factoryMock.Setup(x => x.GetEntityInterceptorsAsync(It.IsAny<IEntityInterceptorContext>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(interceptors);

            _mockRepository = new(_factoryMock.Object);
        }

        [Test]
        public async Task AddAsync_Should_SetCreatedTimestamp_And_NotModified()
        {
            
            var repo = new InternalMemoryMockRepository<ICustomer, DbCustomer, Customer>(_factoryMock.Object);

            var entity = new Customer();

            var id = (await repo.UpsertAsync(entity, CancellationToken.None)).GetResultOrDefault();

            var stored = (await repo.FindAsync(id, CancellationToken.None)).GetResultOrDefault();

            Assert.That(stored, Is.Not.Null);
            Assert.That(stored!.CreatedTimestampUtc,
                Is.EqualTo(timeProvider.GetUtcNow()));
            Assert.That(stored.ModifiedTimestampUtc, Is.Null);
        }
    }
}
