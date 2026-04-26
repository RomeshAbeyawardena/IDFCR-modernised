using IDFCR.Abstractions.Interceptors.Extensions;
using IDFCR.Abstractions.Interceptors.Processors;
using IDFCR.Abstractions.Results;
using System.Text.Json;

namespace IDFCR.Abstractions.Interceptors.Tests;

internal abstract class TestEntityAuditProcessorBase<TEntity>(string name, ICollection<TestEntityAudit> testEntityAuditEntries) : AuditProcessorBase<TEntity, TEntity>(name)
{
    public virtual Task<object?> LookupAsync(string key, object value, CancellationToken cancellationToken)
    {
        return Task.FromResult<object?>(null);
    }

    public override async Task<IUnitResult> AuditChangesAsync(TEntity oldValue, TEntity newValue, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        TestEntityAudit testAuditEntity = new()
        {
            OldValue = JsonSerializer.Serialize(oldValue),
            NewValue = JsonSerializer.Serialize(newValue)
        };

        testAuditEntity.ChangeDescription = await this.AuditChangeDescriptionAsync(oldValue, newValue, cancellationToken,
            deferredLookupAsyncAction: LookupAsync);

        testEntityAuditEntries.Add(testAuditEntity);
        return UnitResult.Success(UnitAction.Add);

    }
}
