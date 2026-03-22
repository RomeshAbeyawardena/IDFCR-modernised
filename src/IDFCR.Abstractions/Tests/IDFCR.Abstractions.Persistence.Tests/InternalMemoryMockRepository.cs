using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Persistence.Extensions;
using IDFCR.Abstractions.Results.Exceptions;
using Moq;

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

        protected override Task<(IEnumerable<TDb> Data, int TotalRows)> OnGetPagedAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<Guid> OnUpdateAsync(TDb entry, T rawEntry, CancellationToken cancellationToken)
        {
            var foundEntry = entries.Find(x => x.Id == entry.Id) ?? throw new EntityNotFoundException(typeof(TDb), entry.Id);
            foundEntry.Apply(entry);
            return Task.FromResult(foundEntry.Id);
        }
    }
}
