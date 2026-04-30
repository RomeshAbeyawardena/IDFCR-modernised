using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Interceptors.Handlers;
using IDFCR.Abstractions.Interceptors.Interceptors;
using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Persistence;
using IDFCR.Abstractions.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IDFCR.Abstractions.Mediator.Extensions.Pipelines;

/// <summary>
/// Represents a MediatR pipeline that post-processes requests and responses to handle unit of work operations. This pipeline checks if the request implements the IUnitOfWorkRequest interface and if the response indicates a successful operation. If both conditions are met, it commits the changes to the underlying data store by calling SaveChangesAsync on the provided IUnitOfWork instance. This allows for automatic handling of unit of work patterns in MediatR-based applications, ensuring that changes are only committed when appropriate based on the request and response characteristics.
/// </summary>
/// <typeparam name="TRequest">The type of request being processed.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the request.</typeparam>
/// <param name="unitOfWork">The unit of work instance used to commit changes.</param>
/// <param name="timeProvider"></param>
/// <param name="serviceProvider"></param>
/// <param name="logger"></param>
public class UnitOfWorkPostPipelineProcessor<TRequest, TResponse>(IUnitOfWork unitOfWork, TimeProvider timeProvider,
    IServiceProvider serviceProvider, ILogger<UnitOfWorkPostPipelineProcessor<TRequest, TResponse>> logger) : MediatR.Pipeline.IRequestPostProcessor<TRequest, TResponse>
    where TRequest : notnull
{
    private async Task NotifyAsync(IOutboxEntity entity, CancellationToken cancellationToken)
    {
        IOutboxEntityNotificationHandler? outboxProcessor = serviceProvider.GetService<IOutboxEntityNotificationHandler>();
        IScopedResources? scopedResources = serviceProvider.GetService<IScopedResources>();

        logger.LogInformation("Notifying outbox pattern:");

        if (outboxProcessor is not null && scopedResources is not null)
        {
            outboxProcessor.ScopedResources = scopedResources;
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Scoped resources count: {count}", scopedResources.Items.Count);
            }

            var outboxEntity = outboxProcessor.Map(entity);
            if (scopedResources.TryGetScopedResource<IIdentifiable>(out var id))
            {
                await outboxProcessor.NotifyAsync(id, outboxEntity, cancellationToken);
            }
            else
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Unable to get outbox ID");
                }
            }
        }
        else
        {
            bool hasOutboxProcessor = outboxProcessor is not null;
            bool hasScopedResources = scopedResources is not null;
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(@"Outbox pattern not supported: 
                Has OutboxProcessor: {hasOutboxProcessor}
                Has ScopedResources: {hasScopedResources}", hasOutboxProcessor, hasScopedResources);
            }
        }
    }

    /// <summary>
    /// Processes the request and response after the main handler has executed. If the request implements IUnitOfWorkRequest and indicates that changes should be committed, and if the response indicates a successful operation, it calls SaveChangesAsync on the unit of work to persist changes to the data store.
    /// </summary>
    /// <param name="request">The request being processed.</param>
    /// <param name="response">The response returned by the request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        if (request is IUnitOfWorkRequest workRequest && workRequest.CommitChanges)
        {
            if (response is IUnitResult unitResult && unitResult.IsSuccess)
            {
                try
                {
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    await NotifyAsync(new DefaultOutboxEntity
                    {
                        CompletedTimestampUtc = timeProvider.GetUtcNow(),
                        ModifiedTimestampUtc = timeProvider.GetUtcNow()
                    }, cancellationToken);
                }
                catch (Exception)
                {
                    await NotifyAsync(new DefaultOutboxEntity
                    {
                        FailedTimestampUtc = timeProvider.GetUtcNow(),
                        ModifiedTimestampUtc = timeProvider.GetUtcNow()
                    }, cancellationToken);

                    throw;
                }
            }
        }
    }
}
