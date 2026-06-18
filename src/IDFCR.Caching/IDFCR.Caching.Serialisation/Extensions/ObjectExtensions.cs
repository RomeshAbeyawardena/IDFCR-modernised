namespace IDFCR.Caching.Serialisation.Extensions;

/// <summary>
/// Defines extension methods for serializing and deserializing objects using MessagePack.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Serialises an object of type T to a byte array using MessagePack with the specified options and cancellation token.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialise.</typeparam>
    /// <param name="value">The object to serialise.</param>
    /// <param name="options">The MessagePack serializer options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the serialised byte array.</returns>
    public static async Task<byte[]> SerialiseAsync<T>(this T value, MessagePack.MessagePackSerializerOptions options, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();

        await MessagePack.MessagePackSerializer.SerializeAsync(memoryStream,
            value, options, cancellationToken);

        var array = memoryStream.ToArray();
        return array;
    }

    /// <summary>
    /// Deserialises a byte array to an object of type T using MessagePack with the specified options and cancellation token.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="byteValue">The byte array to deserialize.</param>
    /// <param name="options">The MessagePack serializer options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object.</returns>
    public static async Task<T> DeserialiseAsync<T>(this IEnumerable<byte> byteValue, MessagePack.MessagePackSerializerOptions options, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream([..byteValue]);
        return await MessagePack.MessagePackSerializer
            .DeserializeAsync<T>(memoryStream, options, cancellationToken);
    }
}
