namespace IDFCR.Abstractions.Cli.Extensions;

public sealed class KeyNotFoundException(string messsage, Exception? innerException) : Exception(messsage, innerException)
{
    public KeyNotFoundException(string? key = null) : this(string.IsNullOrWhiteSpace(key) 
        ? "Key not found" : $"Key '{key}' not found", null) { }
}
