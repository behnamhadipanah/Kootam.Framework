using Kootam.Authentication.Abstractions.Enums;
using Kootam.Authentication.Abstractions.Models;

namespace Kootam.Authentication.Abstractions.Services;

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
