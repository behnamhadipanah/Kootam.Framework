using Kootam.Authentication.Abstractions.Enums;

namespace Kootam.Authentication.SignIn.Models;

public sealed record ContactOtpCacheEntry(
    ContactOtpPurpose Purpose,
    string Destination,
    string CodeHash,
    DateTimeOffset ExpiresAt,
    int FailedAttempts);
