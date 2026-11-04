using Kootam.Authentication.SignIn.Enums;

namespace Kootam.Authentication.SignIn.Models;

public sealed record ExternalIdentity(
    ExternalSignInProvider Provider,
    string Subject,
    string? Email,
    bool EmailVerified,
    string? FirstName,
    string? LastName,
    string? SuggestedUserName);
