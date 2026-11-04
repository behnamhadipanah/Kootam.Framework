using Kootam.Authentication.Abstractions.Models;

namespace Kootam.Authentication.Abstractions.Services;

public interface IExternalIdentityVerifier
{
    Task<ExternalVerificationResult> VerifyAsync(
        ExternalSignInRequest request,
        CancellationToken cancellationToken = default);
}
