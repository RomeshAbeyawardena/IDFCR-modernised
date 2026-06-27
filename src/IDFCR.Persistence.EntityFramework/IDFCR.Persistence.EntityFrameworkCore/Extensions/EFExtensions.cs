using IDFCR.Abstractions.Results;
using Microsoft.EntityFrameworkCore;

namespace IDFCR.Persistence.EntityFrameworkCore.Extensions;

/// <summary>
/// Defines extension methods for Entity Framework Core.
/// </summary>
public static class EFExtensions
{
    /// <summary>
    /// Executes a query to retrieve entities of type <typeparamref name="TSource"/> from the specified <paramref name="query"/>, maps them to results of type <typeparamref name="TDestination"/> using the provided <paramref name="mapperFactory"/>, and returns a collection of unit results. An optional action can be invoked upon materialization of the results.
    /// </summary>
    /// <typeparam name="TSource">The type of the source entity.</typeparam>
    /// <typeparam name="TDestination">The type of the destination result.</typeparam>
    /// <param name="query">The query to execute.</param>
    /// <param name="mapperFactory">The factory function to map the source entity to the destination result.</param>
    /// <param name="invokeOnMaterialisation">An optional action to invoke upon materialization of the results.</param>
    /// <param name="namedResult">An optional name for the result.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the unit result collection of the operation.</returns>
    public static async Task<IUnitResultCollection<TDestination>> ToUnitResultCollectionAsync<TSource, TDestination>(
       this IQueryable<TSource> query,
       Func<TSource, TDestination> mapperFactory,
       Action<IUnitResultCollection<TDestination>>? invokeOnMaterialisation,
       string? namedResult,
       CancellationToken cancellationToken)
    {
        var results = await query.ToArrayAsync(cancellationToken);

        var response = UnitResultCollection.FromResult(results.Select(mapperFactory), namedResult: namedResult);

        invokeOnMaterialisation?.Invoke(response);

        return response;
    }

    /// <summary>
    /// Executes a query to retrieve entities of type <typeparamref name="TSource"/> from the specified <paramref name="query"/>, maps them to results of type <typeparamref name="TDestination"/> using the provided <paramref name="mapperFactory"/>, and returns a collection of unit results.
    /// </summary>
    /// <typeparam name="TSource">The type of the source entity.</typeparam>
    /// <typeparam name="TDestination">The type of the destination result.</typeparam>
    /// <param name="query">The query to execute.</param>
    /// <param name="mapperFactory">The factory function to map the source entity to the destination result.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the unit result collection of the operation.</returns>
    public static Task<IUnitResultCollection<TDestination>> ToUnitResultCollectionAsync<TSource, TDestination>(
       this IQueryable<TSource> query,
       Func<TSource, TDestination> mapperFactory,
       CancellationToken cancellationToken)
    {
        return query.ToUnitResultCollectionAsync(mapperFactory, null, null, cancellationToken);
    }

    /// <summary>
    /// Executes a query to retrieve the first or default entity of type <typeparamref name="TSource"/> from the specified <paramref name="query"/>, applies a filter, and maps it to a result of type <typeparamref name="TDestination"/> using the provided <paramref name="mapperFactory"/>. If no entity is found, a not found result is returned.
    /// </summary>
    /// <typeparam name="TSource">The type of the source entity.</typeparam>
    /// <typeparam name="TDestination">The type of the destination result.</typeparam>
    /// <param name="query">The query to execute.</param>
    /// <param name="filter">The filter to apply.</param>
    /// <param name="mapperFactory">The factory function to map the source entity to the destination result.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the unit result of the operation.</returns>
    public static Task<IUnitResult<TDestination>> FirstOrDefaultToUnitResultAsync<TSource, TDestination>(
       this IQueryable<TSource> query,
       object filter,
       Func<TSource, TDestination> mapperFactory,
       CancellationToken cancellationToken)
    {
        return query.FirstOrDefaultToUnitResultAsync(filter, mapperFactory, null, null, null, cancellationToken);
    }

    /// <summary>
    /// Executes a query to retrieve the first or default entity of type <typeparamref name="TSource"/> from the specified <paramref name="query"/>, applies a filter, and maps it to a result of type <typeparamref name="TDestination"/> using the provided <paramref name="mapperFactory"/>. If no entity is found, a not found result is returned. Additionally, optional actions can be invoked when an entity is found or not found.
    /// </summary>
    /// <typeparam name="TSource">The type of the source entity.</typeparam>
    /// <typeparam name="TDestination">The type of the destination result.</typeparam>
    /// <param name="query">The query to execute.</param>
    /// <param name="filter">The filter to apply.</param>
    /// <param name="mapperFactory">The factory function to map the source entity to the destination result.</param>
    /// <param name="invokeOnNotFound">An optional action to invoke when no entity is found.</param>
    /// <param name="invokeOnFound">An optional action to invoke when an entity is found.</param>
    /// <param name="namedResult">An optional name for the result.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the unit result of the operation.</returns>
    public static async Task<IUnitResult<TDestination>> FirstOrDefaultToUnitResultAsync<TSource, TDestination>(
        this IQueryable<TSource> query,
        object filter,
        Func<TSource, TDestination> mapperFactory,
        Action<IUnitResult<TDestination>>? invokeOnNotFound,
        Action<IUnitResult<TDestination>>? invokeOnFound,
        string? namedResult,
        CancellationToken cancellationToken)
    {
        IUnitResult<TDestination> response;

        var result = await query.FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            response = UnitResult.NotFound<TDestination>(filter);
            invokeOnNotFound?.Invoke(response);

            return response;
        }

        response = UnitResult.FromResult(mapperFactory(result), namedResult: namedResult);
        invokeOnFound?.Invoke(response);
        return response;
    }
}
