using Kootam.Authentication.Abstractions.Enums;

namespace Kootam.Authentication.Abstractions.Models;

public sealed record ExternalIdentity(
    ExternalSignInProvider Provider,
    string Subject,
    string? Email,
    bool EmailVerified,
    string? FirstName,
    string? LastName,
    string? SuggestedUserName);
