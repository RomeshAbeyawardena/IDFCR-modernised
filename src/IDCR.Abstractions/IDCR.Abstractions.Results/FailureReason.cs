namespace IDCR.Abstractions.Results;

public enum FailureReason
{
    Unknown = 99,
    None = 0,
    NotFound = 1,
    Conflict = 2,
    ValidationError = 3,
    Unauthorized = 4,
    Forbidden = 5,
    InternalError = 6
}
