using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Mediator.Extensions;

/// <summary>
/// Represents the behavior to be applied when an exception of a specific type is caught during the handling of a MediatR request. This record encapsulates the action to be taken (UnitAction) and the reason for the failure (FailureReason) when an exception occurs. The UnitAction indicates how the system should respond to the exception, while the FailureReason provides context about why the operation failed. This allows for a standardized way to handle exceptions and communicate failure reasons across different parts of the application.
/// </summary>
/// <param name="UnitAction"></param>
/// <param name="FailureReason"></param>
public record ExceptionBehaviour(UnitAction UnitAction, FailureReason FailureReason);
