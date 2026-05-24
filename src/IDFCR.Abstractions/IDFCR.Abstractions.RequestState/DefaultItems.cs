namespace IDFCR.Abstractions.RequestState;

internal class DefaultItems(IDictionary<string, object?> items, Func<object, string> getItemValue) : StateDictionaryBase(items, getItemValue), IItems
{

}