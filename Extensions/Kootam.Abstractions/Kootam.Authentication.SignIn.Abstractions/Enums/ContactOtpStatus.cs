namespace Kootam.Authentication.SignIn.Enums;

public enum ContactOtpStatus
{
    Success = 1,
    InvalidDestination = 2,
    RateLimited = 3,
    DeliveryFailed = 4,
    InvalidCode = 5,
    Disabled = 6
}
