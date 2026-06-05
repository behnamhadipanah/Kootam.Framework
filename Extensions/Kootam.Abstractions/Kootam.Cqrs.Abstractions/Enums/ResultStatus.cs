namespace Kootam.Cqrs.Abstractions.Enums;

public enum ResultStatus
{
    Success,
    ValidationError,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    Error
}