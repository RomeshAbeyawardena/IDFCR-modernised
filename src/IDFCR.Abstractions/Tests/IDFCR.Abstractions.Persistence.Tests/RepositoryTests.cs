using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Interceptors.Interceptors;
using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Results.Extensions;
using LinqKit;
using Microsoft.Extensions.Time.Testing;
using NUnit.Framework;
using System.Linq.Expressions;

namespace IDFCR.Abstractions.Persistence.Tests
{
    internal record PagedCustomerRequest : PagedQuery
    {
        public string? NameContains { get; set; }
    }

    internal class PagedCustomerFilter : PagedFilterBase<PagedCustomerRequest, DbCustomer>
    {
        protected override Expression<Func<DbCustomer, bool>> BuildPredicate(IQueryable<DbCustomer> queryable, PagedCustomerRequest request)
        {
            var expression = base.StarterExpression;
            var innerExpression =  base.StarterExpression;

            if (!string.IsNullOrWhiteSpace(request.NameContains))
            {
                innerExpression.And(x => x.FirstName.Contains(request.NameContains));
                innerExpression.Or(x => string.IsNullOrWhiteSpace(x.MiddleName) || x.MiddleName.Contains(request.NameContains));
                innerExpression.Or(x => x.LastName.Contains(request.NameContains));
            }

            return expression.And(innerExpression);
        }
    }

    [TestFixture]
    public class RepositoryTests
    {
        private DefaultFilterFactory _defaultFilterFactory;
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

            _defaultFilterFactory = new([new PagedCustomerFilter()]);
            
            _mockRepository = new(_factory, _defaultFilterFactory);
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

            Assert.That(updateResult.IsSuccess, Is.True, $"Internal error: {updateResult.Exception?.Message}");

            var updated = (await _mockRepository.FindAsync(id, CancellationToken.None)).GetResultOrDefault();
            Assert.That(updated, Is.Not.Null);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(updated.CreatedTimestampUtc, Is.EqualTo(originalCreated));
                Assert.That(updated.ModifiedTimestampUtc, Is.EqualTo(_timeProvider.GetUtcNow()));
            }
        }
    }
}
