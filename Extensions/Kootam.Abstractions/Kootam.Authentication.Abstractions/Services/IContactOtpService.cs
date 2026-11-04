using Kootam.Authentication.SignIn.Enums;
using Kootam.Authentication.SignIn.Models;

namespace Kootam.Authentication.SignIn.Contracts;

public interface IContactOtpService
{
    Task<ContactOtpResult> RequestAsync(
        ContactOtpPurpose purpose,
        string destination,
        CancellationToken cancellationToken = default);

    Task<ContactOtpResult> VerifyAsync(
        ContactOtpPurpose purpose,
        string destination,
        string code,
        CancellationToken cancellationToken = default);
}
