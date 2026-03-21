namespace IDFCR.Abstractions.Metadata;

public interface IIdentifiable<TKey>
    where TKey : struct
{
    public TKey Id { get; set; }
}
