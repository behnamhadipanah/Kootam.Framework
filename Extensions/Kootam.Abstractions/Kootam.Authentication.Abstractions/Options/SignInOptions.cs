using Kootam.Authentication.Abstractions.Enums;

namespace Kootam.Authentication.Abstractions.Options;

public sealed class SignInOptions
{
    public const string SectionName = "SignIn";

    public ContactOtpMethodOptions EmailOtp { get; set; } = new()
    {
        Enabled = true,
        Subject = "Sign-in code",
        Body = "Use this code to continue."
    };

    public ContactOtpMethodOptions MobileOtp { get; set; } = new() { Enabled = true };

    public int MaxRequestsPerWindow { get; set; } = 5;

    public int RequestWindowMinutes { get; set; } = 10;

    public int MaxVerificationAttempts { get; set; } = 5;

    public ExternalSignInOptions External { get; set; } = new();

    public bool IsEnabled(ContactOtpPurpose purpose)
        => purpose switch
        {
            ContactOtpPurpose.EmailLogin => EmailOtp.Enabled,
            ContactOtpPurpose.MobileLogin => MobileOtp.Enabled,
            _ => false
        };

    public ContactOtpMethodOptions Method(ContactOtpPurpose purpose)
        => purpose switch
        {
            ContactOtpPurpose.EmailLogin => EmailOtp,
            ContactOtpPurpose.MobileLogin => MobileOtp,
            _ => throw new ArgumentOutOfRangeException(nameof(purpose), purpose, null)
        };
}
