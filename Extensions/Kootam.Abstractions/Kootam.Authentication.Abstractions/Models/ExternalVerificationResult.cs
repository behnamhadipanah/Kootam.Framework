using Kootam.Authentication.Abstractions.Enums;

namespace Kootam.Authentication.Abstractions.Models;

public sealed record ExternalVerificationResult(ExternalVerificationStatus Status, ExternalIdentity? Identity)
{
    public bool Succeeded => Status == ExternalVerificationStatus.Success && Identity is not null;

    public static ExternalVerificationResult Success(ExternalIdentity identity)
        => new(ExternalVerificationStatus.Success, identity);

    public static ExternalVerificationResult Fail(ExternalVerificationStatus status)
        => new(status, null);
}
