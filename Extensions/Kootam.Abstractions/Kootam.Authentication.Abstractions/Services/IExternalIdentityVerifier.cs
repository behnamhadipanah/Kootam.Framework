using Kootam.Authentication.SignIn.Models;

namespace Kootam.Authentication.SignIn.Contracts;

public interface IExternalIdentityVerifier
{
    Task<ExternalVerificationResult> VerifyAsync(
        ExternalSignInRequest request,
        CancellationToken cancellationToken = default);
}
