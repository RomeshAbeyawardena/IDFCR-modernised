using System.IO.Pipelines;

namespace IDFCR.Results.Http.Tests;

internal class TestPipeWriter : PipeWriter
{
    public override long UnflushedBytes => 0;
    public override void Advance(int bytes) { }
    public override void CancelPendingFlush() { }
    public override bool CanGetUnflushedBytes => true;
    public override void Complete(Exception? exception = null) { }
    public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default) => new(new FlushResult { });
    public override Memory<byte> GetMemory(int sizeHint = 0) => new byte[sizeHint > 0 ? sizeHint : 4096];
    public override Span<byte> GetSpan(int sizeHint = 0) => new byte[sizeHint > 0 ? sizeHint : 4096].AsSpan();
}
