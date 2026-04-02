namespace IDFCR.Abstractions.Mapper;

/// <summary>
/// Represents a mapper that maps from a source type to a target type, while also providing information about the mapping state. The IRecordMapper interface extends the IMapper interface, allowing for the mapping of data from a source type to a target type, while also exposing the MappingState property to indicate the current state of the mapping process. This can be useful for tracking the progress of mapping operations and handling any potential issues that may arise during the mapping process, providing developers with greater control and visibility over the mapping operations within applications and systems that utilize mapping mechanisms.
/// </summary>
/// <typeparam name="TSource">The type of the source object to be mapped.</typeparam>
public interface IRecordMapper<TSource> : IMapper<TSource>
{
    /// <summary>
    /// Gets the current state of the mapping process. The MappingState property provides information about the current state of the mapping operation, which can be used to track the progress of mapping operations and handle any potential issues that may arise during the mapping process. By exposing this property, developers can gain greater control and visibility over the mapping operations within applications and systems that utilize mapping mechanisms, allowing for more effective management of mapping processes and improved handling of any issues that may occur during mapping operations.
    /// </summary>
    MappingState MappingState { get; }
}
