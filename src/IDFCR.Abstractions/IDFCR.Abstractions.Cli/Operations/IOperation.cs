namespace IDFCR.Abstractions.Cli.Operations;

public interface IOperation
{
    string Name { get; }
    IEnumerable<string> Aliases { get; }
}
