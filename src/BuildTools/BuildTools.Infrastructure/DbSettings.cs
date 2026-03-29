namespace BuildTools.Infrastructure;

public class DbSettings
{
    public Dictionary<string, string> ConnectionStrings { get; set; } = [];
    public string? DefaultConnectionString { get; set; }
}
