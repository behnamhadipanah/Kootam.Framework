using Kootam.Authentication.Abstractions.Enums;

namespace Kootam.Authentication.SignIn.Services;

internal static class ContactAddress
{
    public static bool TryNormalize(ContactOtpPurpose purpose, string? destination, out string normalized)
    {
        normalized = string.Empty;
        if (purpose == ContactOtpPurpose.EmailLogin)
        {
            if (!IsValidEmail(destination))
                return false;

            normalized = destination!.Trim().ToLowerInvariant();
            return true;
        }

        return TryNormalizeMobile(destination, out normalized);
    }

    private static bool TryNormalizeMobile(string? phoneNumber, out string normalized)
    {
        normalized = string.Empty;
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        var digits = new string(phoneNumber.Where(char.IsAsciiDigit).ToArray());
        if (digits.Length == 0)
            return false;

        if (digits.StartsWith("00", StringComparison.Ordinal))
            digits = digits[2..];
        else if (digits.Length == 11 && digits.StartsWith("09", StringComparison.Ordinal))
            digits = "98" + digits[1..];

        if (digits.Length is < 8 or > 15)
            return false;

        normalized = "+" + digits;
        return true;
    }

    private static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var value = email.Trim();
        if (value.Length > 256 || value.Any(char.IsWhiteSpace))
            return false;

        var at = value.IndexOf('@');
        if (at <= 0 || at != value.LastIndexOf('@'))
            return false;

        var domain = value[(at + 1)..];
        var dot = domain.LastIndexOf('.');
        return dot > 0 && dot < domain.Length - 1;
    }
}
