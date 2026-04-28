using IDFCR.Abstractions.Interceptors.Handlers;

namespace IDFCR.Abstractions.Interceptors.Interceptors;

/// <summary>
/// Represents an interceptor for handling outbox entities, allowing for the processing of outbox messages and the tracking of their status. This class provides a base implementation for intercepting changes to outbox entities, allowing developers to implement custom logic for processing outbox messages within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status. The interceptor is designed to be applied at the Post stage of Insert and Update behaviors, allowing it to capture changes to outbox entities after they have been made, enabling the tracking of message status and the processing of outbox messages based on specific requirements and use cases related to message processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
/// </summary>
public class OutboxInterceptor(IServiceProvider services)
    : EntityInterceptorBase(EntityContextBehaviorStage.Post, EntityContextBehavior.Insert | EntityContextBehavior.Update, 99)
{
    private IOutboxEntityNotificationHandler? _handler;
    /// <inheritdoc />
    public override bool ShouldIntercept(IEntityInterceptorContext context)
    {
        var service = services.GetService(typeof(IOutboxEntityNotificationHandler));

        if(service is null)
        {
            return false;
        }

        _handler = service as IOutboxEntityNotificationHandler;

        return _handler is not null;
    }
#pragma warning disable CS0809
    /// <summary>
    /// Synchronously intercepts the entity changes after they have been made, allowing for the auditing of modifications to the entity. This method is responsible for checking if the context contains a new model of the entity being audited, and if so, it retrieves the audit processor provider to perform the audit of changes between the old and new values of the entity. The implementation of this method can include logic for determining which properties or fields have been modified, capturing the previous and current values, and creating audit entries that can be stored or processed further for compliance, security, or other purposes related to tracking entity modifications within applications and systems that utilize auditing mechanisms for tracking entity modifications.
    /// </summary>
    /// <param name="context">The context of the entity interceptor.</param>
    [Obsolete("This method will not wait and may be cancelled if long-running.", true)]
    public override async void Intercept(IEntityInterceptorContext context)
    {
        await InterceptAsync(context, CancellationToken.None);
    }
#pragma warning restore

    /// <summary>
    /// Invokes the asynchronous interception logic for handling outbox entities, allowing for the processing of outbox messages and the tracking of their status. This method is responsible for checking if the interceptor has a valid handler and if the context contains a model of the entity being processed. If both conditions are met, it invokes the notification handler to process the outbox entity asynchronously, allowing for the handling of outbox messages and the tracking of their status based on specific requirements and use cases related to message processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    /// <param name="context">The context of the entity interceptor.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override async Task InterceptAsync(IEntityInterceptorContext context, CancellationToken cancellationToken)
    {
        if (_handler is null || context.Model is null)
        {
            return;
        }

        _handler.ScopedResources = base.Context?.ScopedResources;
        await _handler.NotifyAsync(context.Model, cancellationToken);
    }
}
