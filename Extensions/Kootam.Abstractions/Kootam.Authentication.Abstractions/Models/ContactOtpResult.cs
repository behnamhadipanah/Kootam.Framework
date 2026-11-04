using Kootam.Authentication.SignIn.Enums;

namespace Kootam.Authentication.SignIn.Models;

public sealed record ContactOtpResult(ContactOtpStatus Status, string? Destination)
{
    public bool Succeeded => Status == ContactOtpStatus.Success;

    public static ContactOtpResult Success(string destination) => new(ContactOtpStatus.Success, destination);

    public static ContactOtpResult Fail(ContactOtpStatus status) => new(status, null);
}
