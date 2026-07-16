using IDFCR.Abstractions.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.Abstractions.Lookups;

internal sealed class DefaultAsyncLookupFactory(IServiceProvider serviceProvider) : IAsyncLookupFactory
{
    private async Task<IEnumerable<TResult>> InScope<TResult, TEntity, TFilter>(TFilter filter,
        Func<IAsyncLookup<TEntity, TFilter>, CancellationToken, Task<TResult?>> resultFactory,
        CancellationToken cancellationToken)
        where TEntity : class
        where TFilter : IFilter
    {
        using var scope = serviceProvider.CreateScope();

        var lookupProviders = scope.ServiceProvider.GetServices<IAsyncLookup<TEntity, TFilter>>();
        List<TResult> collectiveResults = [];

        foreach (var lookup in lookupProviders)
        {
            if (!await lookup.CanLookupAsync(filter, cancellationToken))
            {
                continue;
            }

            var result = await resultFactory(lookup, cancellationToken);

            if (result is not null)
            {
                collectiveResults.Add(result);
            }
        }

        return collectiveResults;
    }

    private async Task<IEnumerable<TResult>> InScope<TResult, TEntity>(object? filter,
        Func<IAsyncLookup<TEntity>, CancellationToken, Task<TResult?>> resultFactory, 
        CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var lookupProviders = scope.ServiceProvider.GetServices<IAsyncLookup<TEntity>>();
        List<TResult> collectiveResults = [];

        foreach (var lookup in lookupProviders)
        {
            if (!await lookup.CanLookupAsync(filter, cancellationToken))
            {
                continue;
            }

            var result = await resultFactory(lookup, cancellationToken);

            if (result is not null)
            {
                collectiveResults.Add(result);
            }
        }

        return collectiveResults;
    }

    public async Task<bool> HasAsync<TEntity>(object? filter, CancellationToken cancellationToken)
    {
        var results = await InScope<bool, TEntity>(filter, async (provider, ct) =>
        {
            return await provider.HasAsync(filter, ct);
        }, cancellationToken);

        return results.Any(x => x);
    }

    public async Task<bool> HasAsync<TEntity, TFilter>(TFilter filter, CancellationToken cancellationToken)
        where TEntity : class
        where TFilter : IFilter
    {
        var results = await InScope<bool, TEntity, TFilter>(filter, async (provider, ct) =>
        {
            return await provider.HasAsync(filter, ct);
        }, cancellationToken);

        return results.Any(x => x);
    }
    
    public async Task<IEnumerable<TEntity>> LookupAsync<TEntity>(object filter, CancellationToken cancellationToken)
        where TEntity : class
    {
        var results = await InScope<TEntity, TEntity>(filter, async (provider, ct) =>
        {
            return await provider.LookupAsync(filter, ct);
        }, cancellationToken);

        return results;
    }

    public async Task<IEnumerable<TEntity>> LookupAsync<TEntity, TFilter>(TFilter filter, CancellationToken cancellationToken)
        where TEntity : class
        where TFilter : IFilter
    {
        var results = await InScope<TEntity, TEntity, TFilter>(filter, async (provider, ct) =>
        {
            return await provider.LookupAsync(filter, ct);
        }, cancellationToken);

        return results;
    }
}