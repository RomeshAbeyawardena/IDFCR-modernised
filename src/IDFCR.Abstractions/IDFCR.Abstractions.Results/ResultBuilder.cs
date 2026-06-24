namespace IDFCR.Abstractions.Results;

/// <summary>
/// Defines a static class that provides methods for building instances of <see cref="IUnitResult"/> and <see cref="IUnitResult{T}"/> with various configurations. The ResultBuilder class serves as a factory for creating result objects, allowing for customization of properties such as success status, action, exception, failure reason, failure origin, and additional metadata. It also supports building results from existing result instances and provides a mechanism for setting modified state in derived result types.
/// </summary>
public static class ResultBuilder
{
    /// <summary>
    /// Builds an instance of <see cref="IUnitResult"/> with the specified parameters and optional configuration. This method allows for the creation of a result object with properties such as success status, action, exception, failure reason, failure origin, and additional metadata. The buildConfiguration parameter provides a way to further customize the result using an <see cref="IUnitResultBuilder"/>.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the result represents a successful operation.</param>
    /// <param name="action">Specifies the action associated with the result.</param>
    /// <param name="exception">Optional exception associated with the result.</param>
    /// <param name="failureReason">Optional reason for failure.</param>
    /// <param name="failureOrigin">Optional origin of the failure.</param>
    /// <param name="buildConfiguration">Optional configuration action for customizing the result.</param>
    /// <returns>An instance of <see cref="IUnitResult"/> with the specified configuration.</returns>
    public static IUnitResult Build(bool isSuccess,
        UnitAction action,
        Exception? exception = null,
        FailureReason? failureReason = null,
        FailureOrigin? failureOrigin = null,
        Action<IUnitResultBuilder>? buildConfiguration = null)
    {
        var builder = new UnitResultBuilder(isSuccess, action, exception, failureReason, failureOrigin);

        buildConfiguration?.Invoke(builder);
        return builder.Build();
    }

    /// <summary>
    /// Builds an instance of <see cref="IUnitResult"/> based on an existing result and optional configuration. This method allows for the creation of a new result object by copying properties from an existing result and applying additional customizations through the buildConfiguration parameter. The resulting <see cref="IUnitResult"/> will reflect the properties of the original result along with any modifications specified in the configuration.
    /// </summary>
    /// <param name="result">The existing result to base the new result on.</param>
    /// <param name="buildConfiguration">Optional configuration action for customizing the result.</param>
    /// <returns>An instance of <see cref="IUnitResult"/> with the specified configuration.</returns>
    public static IUnitResult Build(IUnitResult result,
        Action<IUnitResultBuilder>? buildConfiguration = null)
    {
        var builder = new UnitResultBuilder(result);

        buildConfiguration?.Invoke(builder);
        return builder.Build();
    }

    /// <summary>
    /// Builds an instance of <see cref="IUnitResult{T}"/> with the specified parameters and optional configuration. This method allows for the creation of a result object with properties such as success status, action, exception, failure reason, failure origin, named result, and additional metadata. The buildConfiguration parameter provides a way to further customize the result using an <see cref="IUnitResultBuilder"/>.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="result">The result value.</param>
    /// <param name="isSuccess">Indicates whether the result represents a successful operation.</param>
    /// <param name="action">Specifies the action associated with the result.</param>
    /// <param name="exception">Optional exception associated with the result.</param>
    /// <param name="failureReason">Optional reason for failure.</param>
    /// <param name="failureOrigin">Optional origin of the failure.</param>
    /// <param name="namedResult">Optional name for the result.</param>
    /// <param name="buildConfiguration">Optional configuration action for customizing the result.</param>
    /// <returns>An instance of <see cref="IUnitResult{T}"/> with the specified configuration.</returns>
    public static IUnitResult<T> Build<T>(
        T? result,
        bool isSuccess,
        UnitAction action,
        Exception? exception = null,
        FailureReason? failureReason = null,
        FailureOrigin? failureOrigin = null,
        string? namedResult = null,
        Action<IUnitResultBuilder>? buildConfiguration = null)
    {
        var builder = new UnitResultBuilder<T>(isSuccess, action, exception, failureReason, namedResult, failureOrigin, default);

        buildConfiguration?.Invoke(builder);
        return builder.Build(result);
    }

    /// <summary>
    /// Builds an instance of <see cref="IUnitResult{T}"/> based on an existing result and optional configuration. This method allows for the creation of a new result object by copying properties from an existing result and applying additional customizations through the buildConfiguration parameter. The resulting <see cref="IUnitResult{T}"/> will reflect the properties of the original result along with any modifications specified in the configuration.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="result">The existing result to base the new result on.</param>
    /// <param name="buildConfiguration">Optional configuration action for customizing the result.</param>
    /// <returns>An instance of <see cref="IUnitResult{T}"/> with the specified configuration.</returns>
    public static IUnitResult<T> Build<T>(IUnitResult<T> result,
        Action<IUnitResultBuilder>? buildConfiguration = null)
    {
        var builder = new UnitResultBuilder<T>(result);

        buildConfiguration?.Invoke(builder);
        return builder.Build(result.Result);
    }
}
