using Kootam.Authentication.SignIn.Enums;

namespace Kootam.Authentication.SignIn.Models;

public sealed class SignInDevelopmentCodes
{
    public string? LastEmailCode { get; private set; }

    public string? LastSmsCode { get; private set; }

    public void Remember(ContactOtpPurpose purpose, string code)
    {
        if (purpose == ContactOtpPurpose.EmailLogin)
            LastEmailCode = code;
        else
            LastSmsCode = code;
    }
}
