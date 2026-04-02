namespace IDFCR.Abstractions.Cli.Extensions;

/// <summary>
/// Represents a decorator for console colors that can be used to temporarily change the background and foreground colors of the console output. When an instance of this class is created, it sets the console colors to the specified values, and when the instance is disposed, it resets the console colors back to their original values. This allows for easy management of console colors in a scoped manner, ensuring that any changes to the console colors are properly cleaned up after use.
/// </summary>
public class ConsoleColourDecorator : IDisposable
{
    private readonly ConsoleColor? resetBackgroundColour;
    private readonly ConsoleColor? resetForegroundColour;

    /// <summary>
    /// Defines a new instance of the ConsoleColourDecorator class, which sets the console background and foreground colors to the specified values. If a color value is not provided (i.e., null), the corresponding console color will not be changed. The original console colors are stored so that they can be restored when the instance is disposed.
    /// </summary>
    /// <param name="backgroundColour">The background color to set. If null, the background color will not be changed.</param>
    /// <param name="foregroundColour">The foreground color to set. If null, the foreground color will not be changed.</param>
    public ConsoleColourDecorator(ConsoleColor? backgroundColour, ConsoleColor? foregroundColour)
    {
        if (backgroundColour.HasValue)
        {
            resetBackgroundColour = Console.BackgroundColor;
            Console.BackgroundColor = backgroundColour.Value;
        }

        if (foregroundColour.HasValue)
        {
            resetForegroundColour = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColour.Value;
        }
        
        
    }

    /// <summary>
    /// Disposes of the ConsoleColourDecorator instance, resetting the console background and foreground colors to their original values if they were changed. This method should be called when the console color changes are no longer needed, typically at the end of a using block or when the instance goes out of scope. The GC.SuppressFinalize method is called to prevent the finalizer from running, as there are no unmanaged resources to clean up.
    /// </summary>
    public void Dispose()
    {
        if (resetBackgroundColour.HasValue)
        {
            Console.BackgroundColor = resetBackgroundColour.Value;
        }

        if (resetForegroundColour.HasValue)
        {
            Console.ForegroundColor = resetForegroundColour.Value;
        }

        GC.SuppressFinalize(this);
    }
}
