namespace BuildTools.Cli;

public record Parameter(string Key, string? Value = null, bool IsFlag = false);
