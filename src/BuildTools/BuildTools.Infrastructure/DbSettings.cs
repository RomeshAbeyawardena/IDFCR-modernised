namespace BuildTools.Infrastructure;

public class DbSettings
{
    public Dictionary<string, string> ConnectionStrings { get; set; } = [];
    public string? DefaultConnectionString { get; set; }
    public string? Server { get; set; }
    public string? InitialCatalog { get; set; }
    public string? UserId { get; set; }
    public string? Password { get; set; }
}
