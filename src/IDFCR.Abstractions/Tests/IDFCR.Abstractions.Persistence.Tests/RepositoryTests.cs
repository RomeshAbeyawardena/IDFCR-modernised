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

        private DefaultEntityInterceptorFactory _factory;
        private InternalMemoryMockRepository<ICustomer, DbCustomer, Customer> _mockRepository;
        private FakeTimeProvider _timeProvider;
        [SetUp]
        public void SetUp()
        {
            _timeProvider = new FakeTimeProvider(
                new DateTimeOffset(2025, 03, 1, 10, 40, 0, TimeSpan.Zero));


            _factory = new([
                new AuditCreatedTimestampEntityInterceptor(_timeProvider),
                new AuditModifiedTimestampEntityInterceptor(_timeProvider)
            ]);
            
            _mockRepository = new(_factory);
        }

        [Test]
        public async Task AddAsync_Should_SetCreatedTimestamp_And_NotModified()
        {
            var entity = new Customer();

            var upsertResult = await _mockRepository.UpsertAsync(entity, CancellationToken.None);

            Assert.That(upsertResult.IsSuccess, Is.True, $"Internal error: {upsertResult.Exception?.Message}");

            var id = upsertResult.GetResultOrDefault();

            var stored = (await _mockRepository.FindAsync(id, CancellationToken.None)).GetResultOrDefault();

            Assert.That(stored, Is.Not.Null);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(stored!.CreatedTimestampUtc,
                            Is.EqualTo(_timeProvider.GetUtcNow()));
                Assert.That(stored.ModifiedTimestampUtc, Is.Null);
            }
        }

        [Test]
        public async Task UpsertAsync_Update_Should_SetModifiedTimestamp_And_PreserveCreated()
        {
            var entity = new Customer();

            var insertResult = await _mockRepository.UpsertAsync(entity, CancellationToken.None);
            var id = insertResult.GetResultOrDefault();

            var stored = (await _mockRepository.FindAsync(id, CancellationToken.None)).GetResultOrDefault();

            var originalCreated = stored!.CreatedTimestampUtc;

            _timeProvider.Advance(TimeSpan.FromHours(1));

            var updateResult = await _mockRepository.UpsertAsync(stored, CancellationToken.None);

            Assert.That(updateResult.IsSuccess, Is.True);

            var updated = (await _mockRepository.FindAsync(id, CancellationToken.None)).GetResultOrDefault();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(updated!.CreatedTimestampUtc, Is.EqualTo(originalCreated));
                Assert.That(updated.ModifiedTimestampUtc, Is.EqualTo(_timeProvider.GetUtcNow()));
            }
        }
    }
}
