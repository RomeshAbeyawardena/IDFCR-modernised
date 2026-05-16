namespace IDFCR.DatabaseUpdater.Extensions;

internal sealed class ConfiguredDatabaseUpdaterHost(IDisposable disposable) : IConfiguredDatabaseUpdaterHost
{
    public int? CommandResult { get; init; }

    public void Dispose()
    {
        disposable.Dispose();
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}
