using IDFCR.Abstractions.Interceptors.Factories;
using IDFCR.Abstractions.Interceptors.Processors;
using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Interceptors.Providers;

internal class DefaultAuditProcessorProvider(IEnumerable<IAuditProcessor> auditProcessors) : IAuditProcessorProvider
{
    public IEntityInterceptorFactory? InterceptorFactory { get; set; }

    public async Task<IUnitResult> AuditChangesAsync(string entityName, object oldValue, object newValue, CancellationToken cancellationToken)
    {
        IAuditProcessor[] processors = [.. auditProcessors.Where(x => x.EntityName == entityName)];
        if (processors.Length > 1)
        {
            return UnitResult.Failed(new InvalidOperationException($"Multiple {nameof(IAuditProcessor)}'s found for '{entityName}'"), UnitAction.Get, FailureReason.Conflict);
        }

        if (processors.Length < 1)
        {
            //nothing to process and there are no issues with this.
            return UnitResult.Success(UnitAction.None);
        }

        var processor = processors[0];

        processor.Provider = this;
        return await processor.AuditChangesAsync(oldValue, newValue, cancellationToken);
    }
}