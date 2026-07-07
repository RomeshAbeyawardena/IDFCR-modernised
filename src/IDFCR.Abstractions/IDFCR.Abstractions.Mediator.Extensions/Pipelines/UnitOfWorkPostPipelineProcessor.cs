using IDFCR.Abstractions.DependencyInjection;
using IDFCR.Abstractions.Outbox;
using IDFCR.Abstractions.Outbox.Handlers;
using IDFCR.Abstractions.Persistence;
using IDFCR.Abstractions.Results;
using IDFCR.Utilities.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IDFCR.Abstractions.Mediator.Extensions.Pipelines;

/// <summary>
/// Represents a MediatR pipeline that post-processes requests and responses to handle unit of work operations. This pipeline checks if the request implements the IUnitOfWorkRequest interface and if the response indicates a successful operation. If both conditions are met, it commits the changes to the underlying data store by calling SaveChangesAsync on the provided IUnitOfWork instance. This allows for automatic handling of unit of work patterns in MediatR-based applications, ensuring that changes are only committed when appropriate based on the request and response characteristics.
/// </summary>
/// <typeparam name="TRequest">The type of request being processed.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the request.</typeparam>
/// <param name="unitOfWork">The unit of work instance used to commit changes.</param>
/// <param name="timeProvider">The time provider used to obtain the current time.</param>
/// <param name="serviceProvider">The service provider used to resolve dependencies.</param>
/// <param name="logger">The logger used to log information and errors.</param>
public class UnitOfWorkPostPipelineProcessor<TRequest, TResponse>(IUnitOfWork unitOfWork, TimeProvider timeProvider,
    IServiceProvider serviceProvider, ILogger<UnitOfWorkPostPipelineProcessor<TRequest, TResponse>> logger) : MediatR.Pipeline.IRequestPostProcessor<TRequest, TResponse>
    where TRequest : notnull
{
    private async Task ProcessOutbox(Func<IOutboxEntityNotificationHandler, IScopedResources, CancellationToken, Task> processOutbox, CancellationToken cancellationToken)
    {
        IOutboxEntityNotificationHandler? outboxProcessor = serviceProvider.GetService<IOutboxEntityNotificationHandler>();
        IScopedResources? scopedResources = serviceProvider.GetService<IScopedResources>();

        if (outboxProcessor is not null && scopedResources is not null)
        {
            outboxProcessor.ScopedResources = scopedResources;

            logger.LogMethod(LogLevel.Information, "Scoped resources count: {count}", args: scopedResources.Items.Count);

            await processOutbox(outboxProcessor, scopedResources, cancellationToken);
        }
        else
        {
            bool hasOutboxProcessor = outboxProcessor is not null;
            bool hasScopedResources = scopedResources is not null;

            logger.LogMethod(LogLevel.Information, @"Outbox pattern not supported: 
                Has OutboxProcessor: {hasOutboxProcessor}
                Has ScopedResources: {hasScopedResources}", args: [hasOutboxProcessor, hasScopedResources]);
        }
    }

    private async Task NotifyAsync(IOutboxEntity entity, CancellationToken cancellationToken)
    {
        await ProcessOutbox(async (outboxProcessor, s, ct) =>
        {
            var outboxEntity = outboxProcessor.Map(entity);

            logger.LogMethod(LogLevel.Information, "Mapped outbox entity: {entityType}, CompletedTimestampUtc: {completedTimestamp}, FailedTimestampUtc: {failedTimestamp}, ModifiedTimestampUtc: {modifiedTimestamp}", args:
                [outboxEntity.EntityType, outboxEntity.CompletedTimestampUtc, outboxEntity.FailedTimestampUtc, outboxEntity.ModifiedTimestampUtc]);

            await outboxProcessor.NotifyFailureAsync(outboxEntity, cancellationToken);
        }, cancellationToken);
    }

    private async Task UpdateNotificationAsync(IOutboxEntity entity, CancellationToken cancellationToken)
    {
        await ProcessOutbox(async(outboxProcessor, s, ct) => 
        {
            var outboxEntity = outboxProcessor.Map(entity);

            logger.LogMethod(LogLevel.Information, "Mapped outbox entity: {entityType}, CompletedTimestampUtc: {completedTimestamp}, FailedTimestampUtc: {failedTimestamp}, ModifiedTimestampUtc: {modifiedTimestamp}", args:
                [outboxEntity.EntityType, outboxEntity.CompletedTimestampUtc, outboxEntity.FailedTimestampUtc, outboxEntity.ModifiedTimestampUtc]);
            
            await outboxProcessor.UpdateNotificationAsync(outboxEntity, cancellationToken);
        }, cancellationToken);
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
                bool isException = false;
                try
                {
                    try
                    {
                        await unitOfWork.SaveChangesAsync(cancellationToken);

                        await UpdateNotificationAsync(new DefaultOutboxEntity
                        {
                            CompletedTimestampUtc = timeProvider.GetUtcNow(),
                            ModifiedTimestampUtc = timeProvider.GetUtcNow()
                        }, cancellationToken);

                        await unitOfWork.SaveChangesAsync(cancellationToken);
                    }
                    catch (Exception)
                    {
                        isException = true;
                        await NotifyAsync(new DefaultOutboxEntity
                        {
                            FailedTimestampUtc = timeProvider.GetUtcNow(),
                            ModifiedTimestampUtc = timeProvider.GetUtcNow()
                        }, cancellationToken);

                        await unitOfWork.SaveChangesAsync(cancellationToken);

                        throw;
                    }
                    
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while committing changes in the UnitOfWorkPostPipelineProcessor.");
                    if (isException)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
