using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Metadata.Lookups;
using IDFCR.Abstractions.Results;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.Abstractions.Lookups;

internal sealed class DefaultAsyncLookupFactory(IServiceProvider serviceProvider) : IAsyncLookupFactory
{
    private async Task<ILookupResults<TResult>> InScope<TResult, TEntity, TFilter>(TFilter filter,
        Func<IAsyncLookup<TEntity, TFilter>, CancellationToken, Task<IUnitResult<TResult>>> resultFactory,
        CancellationToken cancellationToken)
        where TEntity : class
        where TFilter : IFilter
    {
        using var scope = serviceProvider.CreateScope();

        var lookupProviders = scope.ServiceProvider.GetServices<IAsyncLookup<TEntity, TFilter>>();
        LookupResultsBuilder<TResult> collectiveResults = new();

        foreach (var lookup in lookupProviders)
        {
            if (!await lookup.CanLookupAsync(filter, cancellationToken))
            {
                continue;
            }

            var result = await resultFactory(lookup, cancellationToken);

            if (result is not null)
            {
                collectiveResults.Add(new LookupUnitResult<TResult>(lookup.GetType(), result));
            }
        }

        return new LookupResults<TResult>(collectiveResults.Build());
    }

    private async Task<ILookupResults<TResult>> InScope<TResult, TEntity>(object? filter,
        Func<IAsyncLookup<TEntity>, CancellationToken, Task<IUnitResult<TResult>>> resultFactory, 
        CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var lookupProviders = scope.ServiceProvider.GetServices<IAsyncLookup<TEntity>>();
        LookupResultsBuilder<TResult> collectiveResults = new();

        foreach (var lookup in lookupProviders)
        {
            if (!await lookup.CanLookupAsync(filter, cancellationToken))
            {
                continue;
            }

            var result = await resultFactory(lookup, cancellationToken);

            if (result is not null)
            {
                collectiveResults.Add(new LookupUnitResult<TResult>(lookup.GetType(), result));
            }
        }

        return new LookupResults<TResult>(collectiveResults.Build());
    }

    public async Task<bool> HasAsync<TEntity>(object? filter, CancellationToken cancellationToken)
    {
        var results = await InScope<bool, TEntity>(filter, async (provider, ct) =>
        {
            return await provider.HasAsync(filter, ct);
        }, cancellationToken);

        return results.Results.Any(x => x.Result);
    }

    public async Task<bool> HasAsync<TEntity, TFilter>(TFilter filter, CancellationToken cancellationToken)
        where TEntity : class
        where TFilter : IFilter
    {
        var results = await InScope<bool, TEntity, TFilter>(filter, async (provider, ct) =>
        {
            return await provider.HasAsync(filter, ct);
        }, cancellationToken);

        return results.Results.Any(x => x.Result);
    }
    
    public async Task<ILookupResults<TEntity>> LookupAsync<TEntity>(object filter, CancellationToken cancellationToken)
        where TEntity : class
    {
        var results = await InScope<TEntity, TEntity>(filter, async (provider, ct) =>
        {
            return await provider.LookupAsync(filter, ct);
        }, cancellationToken);

        return results;
    }

    public async Task<ILookupResults<TEntity>> LookupAsync<TEntity, TFilter>(TFilter filter, CancellationToken cancellationToken)
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