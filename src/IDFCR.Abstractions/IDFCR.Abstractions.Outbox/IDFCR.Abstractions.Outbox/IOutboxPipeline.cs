
namespace IDFCR.Abstractions.Outbox;

public interface IOutboxPipeline : IAsyncDisposable
{
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}
