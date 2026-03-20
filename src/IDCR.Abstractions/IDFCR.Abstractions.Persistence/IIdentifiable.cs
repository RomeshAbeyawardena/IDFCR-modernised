namespace IDCR.Abstractions.Persistence;

public interface IIdentifiable<TKey>
    where TKey : struct
{
    public TKey Id { get; set; }
}
