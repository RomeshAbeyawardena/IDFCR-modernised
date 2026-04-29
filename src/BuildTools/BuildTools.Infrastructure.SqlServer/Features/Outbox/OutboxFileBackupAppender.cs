using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BuildTools.Infrastructure.SqlServer.Features.Outbox;

public sealed class OutboxFileBackupAppender(string directoryPath) : IOutboxFileBackupAppender
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly string _directoryPath = string.IsNullOrWhiteSpace(directoryPath)
        ? Path.Combine(AppContext.BaseDirectory, "outbox-replay")
        : directoryPath;

    public async Task AppendAsync(OutboxEntity entity, CancellationToken cancellationToken)
    {
        var payloadJson = JsonSerializer.Serialize(entity, SerializerOptions);
        var payloadHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payloadJson)));

        var envelope = new OutboxBackupEnvelope(
            SchemaVersion: 1,
            WrittenTimestampUtc: DateTimeOffset.UtcNow,
            BackupEventId: Guid.NewGuid(),
            EntityType: typeof(OutboxEntity).FullName ?? nameof(OutboxEntity),
            PayloadJson: payloadJson,
            PayloadSha256: payloadHash);

        var line = JsonSerializer.Serialize(envelope, SerializerOptions);
        var bytes = Encoding.UTF8.GetBytes(line + Environment.NewLine);

        var filePath = Path.Combine(_directoryPath, $"outbox-{DateTime.UtcNow:yyyyMMdd}.ndjson");
        Directory.CreateDirectory(_directoryPath);

        await _gate.WaitAsync(cancellationToken);
        try
        {
            await using var stream = new FileStream(filePath, new FileStreamOptions
            {
                Mode = FileMode.Append,
                Access = FileAccess.Write,
                Share = FileShare.Read,
                Options = FileOptions.Asynchronous | FileOptions.WriteThrough
            });

            await stream.WriteAsync(bytes, cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    private sealed record OutboxBackupEnvelope(
        int SchemaVersion,
        DateTimeOffset WrittenTimestampUtc,
        Guid BackupEventId,
        string EntityType,
        string PayloadJson,
        string PayloadSha256);
}