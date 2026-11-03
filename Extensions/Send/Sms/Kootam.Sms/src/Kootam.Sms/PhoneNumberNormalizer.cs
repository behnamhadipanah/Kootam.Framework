namespace Kootam.Sms;

public static class PhoneNumberNormalizer
{
    public const int MinDigits = 8;
    public const int MaxDigits = 15;
    private const string IranTrunkPrefix = "09";
    private const string IranCountryDigits = "98";

    public static string? Normalize(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return null;

        var digits = new string(phoneNumber.Where(char.IsAsciiDigit).ToArray());
        if (digits.Length == 0)
            return null;

        if (digits.StartsWith("00", StringComparison.Ordinal))
            digits = digits[2..];
        else if (digits.Length == 11 && digits.StartsWith(IranTrunkPrefix, StringComparison.Ordinal))
            digits = IranCountryDigits + digits[1..];

        return "+" + digits;
    }

    public static bool IsValid(string? phoneNumber)
    {
        var normalized = Normalize(phoneNumber);
        if (normalized is null)
            return false;

        var digits = normalized.Length - 1;
        return digits is >= MinDigits and <= MaxDigits;
    }
}
