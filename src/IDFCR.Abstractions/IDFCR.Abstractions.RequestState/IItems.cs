namespace IDFCR.Abstractions.RequestState;

/// <summary>
/// Represents a collection of items associated with an authenticated HTTP request, allowing for the storage and retrieval of arbitrary data related to the request. This interface extends the IStateDictionary, providing a structured way to manage contextual information that may be needed during the processing of the request, such as user information, request metadata, or other relevant data that can be accessed throughout the lifecycle of the request.
/// </summary>
public interface IItems : IStateDictionary
{

}
