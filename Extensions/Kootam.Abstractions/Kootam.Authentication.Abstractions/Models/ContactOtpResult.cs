using Kootam.Authentication.Abstractions.Enums;

namespace Kootam.Authentication.Abstractions.Models;

public sealed record ContactOtpResult(ContactOtpStatus Status, string? Destination)
{
    public bool Succeeded => Status == ContactOtpStatus.Success;

    public static ContactOtpResult Success(string destination) => new(ContactOtpStatus.Success, destination);

    public static ContactOtpResult Fail(ContactOtpStatus status) => new(status, null);
}
