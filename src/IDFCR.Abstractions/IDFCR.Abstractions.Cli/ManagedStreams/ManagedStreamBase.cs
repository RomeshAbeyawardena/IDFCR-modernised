namespace IDFCR.Abstractions.Cli.ManagedStreams;

/// <summary>
/// Represents a base class for managed streams that provides common functionality for accessing standard input, output, and error streams, as well as properties for determining interactivity and stream width. This abstract class serves as a foundation for specific implementations of managed streams, allowing for the encapsulation of shared behavior and properties. The constructor of this class initializes the properties based on the provided parameters, enabling derived classes to easily set up their respective streams and configurations. The ManagedStreamBase class implements the IManagedStream interface, ensuring that all derived classes adhere to the contract defined by the interface while providing flexibility in how the streams are accessed and managed based on the underlying stream type and requirements of the application.
/// </summary>
/// <param name="width">The width of the stream buffer, which can be useful for formatting output or managing display constraints.</param>
/// <param name="error">The error stream, which is used for writing error messages or diagnostics.</param>
/// <param name="in">The standard input stream, which is used for reading input from the user or other sources.</param>
/// <param name="out">The standard output stream, which is used for writing output to the user or other destinations.</param>
/// <param name="newInstance">A function that creates a new instance of a readable stream based on the provided TextReader.</param>
/// <param name="isInteractive">A value indicating whether the stream is interactive.</param>
public abstract class ManagedStreamBase(long width, TextWriter error, TextReader @in, TextWriter @out, Func<TextReader, IIOReadableStream> newInstance, bool isInteractive = false) : IManagedStream
{
    /// <summary>
    /// Gets a value indicating whether the stream is interactive. This property allows for determining if the stream is capable of interacting with the user, such as through console input and output. An interactive stream typically supports reading from standard input and writing to standard output, allowing for user interaction through the console interface. The value of this property can be used to conditionally enable or disable certain features or behaviors based on whether the stream is interactive, providing flexibility in how the application handles different types of streams and user interactions.
    /// </summary>
    public bool IsInteractive { get; } = isInteractive;
    /// <summary>
    /// Gets the error stream, which is used for writing error messages or diagnostics. This property provides access to a writeable stream that can be used to output error information, allowing for separation of error output from standard output. The Error stream can be utilized to log errors, display diagnostic messages, or provide feedback to the user in case of issues or exceptions. Implementations of this interface can provide specific behavior for how the error stream is accessed and managed based on the underlying stream type and requirements of the application.
    /// </summary>
    public IIOWriteableStream Error { get; } = new WriteableStreamBase(error);
    /// <summary>
    /// Gets the standard input stream, which is used for reading input from the user or other sources. This property provides access to a readable stream that can be used to read characters and lines of text, allowing for user interaction through the console interface or other input sources. The In stream can be utilized to capture user input, read data from files, or process input from other sources as needed. Implementations of this interface can provide specific behavior for how the input stream is accessed and managed based on the underlying stream type and requirements of the application.
    /// </summary>
    public IIOReadableStream In { get; } = newInstance(@in);

    /// <summary>
    /// Gets the standard output stream, which is used for writing output to the user or other destinations. This property provides access to a writeable stream that can be used to output text and information, allowing for user interaction through the console interface or other output destinations. The Out stream can be utilized to display information to the user, write data to files, or send output to other destinations as needed. Implementations of this interface can provide specific behavior for how the output stream is accessed and managed based on the underlying stream type and requirements of the application.
    /// </summary>
    public IIOWriteableStream Out { get; } = new WriteableStreamBase(@out);

    /// <summary>
    /// Gets the width of the stream buffer, which can be useful for formatting output or managing display constraints. This property provides information about the width of the stream, allowing for dynamic adjustment of output formatting based on the available width. The Width property can be utilized to ensure that output is properly formatted and displayed within the constraints of the stream, improving readability and user experience. Implementations of this interface can provide specific behavior for how the width is determined and managed based on the underlying stream type and requirements of the application.
    /// </summary>
    public long Width { get; } = width;
}
