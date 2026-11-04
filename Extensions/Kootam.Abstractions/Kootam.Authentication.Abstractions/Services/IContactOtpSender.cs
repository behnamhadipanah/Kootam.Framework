using Kootam.Authentication.SignIn.Enums;

namespace Kootam.Authentication.SignIn.Contracts;

public interface IContactOtpSender
{
    Task SendAsync(
        ContactOtpPurpose purpose,
        string destination,
        string code,
        DateTimeOffset expiresAt,
        CancellationToken cancellationToken = default);
}
