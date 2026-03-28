namespace BuildTools.Cli.StateManagement;

public interface IState<T> : IState
{
    T Value { get; }
    void Update(T value, DateTimeOffset updatedTimestampUtc);
}

public record State<T>(string Name, T OriginalValue, DateTimeOffset CreatedTimestampUtc) : IState<T>
{
    public void Update(T value, DateTimeOffset updatedTimestampUtc)
    {
        Value = value;
        UpdatedTimestampUtc = updatedTimestampUtc;
    }

    public T Value { get; private set; } = OriginalValue;
    public DateTimeOffset? UpdatedTimestampUtc { get; private set; }
}
