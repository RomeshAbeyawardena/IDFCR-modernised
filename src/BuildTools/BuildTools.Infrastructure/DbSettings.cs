namespace BuildTools.Infrastructure;

public class DbSettings
{
    public Dictionary<string, string> ConnectionStrings { get; set; } = [];
    public bool EnableDetailedErrors { get; set; }
    public string? DefaultConnectionStringName { get; set; }
    public string? Server { get; set; }
    public string? InitialCatalog { get; set; }
    public string? UserId { get; set; }
    public string? Password { get; set; }

    // Outbox file backup (for later replay service)
    public bool EnableOutboxFileBackup { get; set; } = true;
    public string? OutboxFileBackupDirectory { get; set; }
}
