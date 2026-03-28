namespace BuildTools.Cli.StateManagement;

public interface IStateLogger<T>
{
    void Append(string name, T initialValue);
    void Update(string name, T value);
}
