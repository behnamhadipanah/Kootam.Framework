namespace Kootam.Sms;

public sealed class SmsOptions
{
    public const string SectionName = "Sms";

    public string Mode { get; set; } = "Log";

    public KavenegarSmsOptions Kavenegar { get; set; } = new();

    public bool IsLog => string.Equals(Mode, "Log", StringComparison.OrdinalIgnoreCase);

    public bool IsKavenegar => string.Equals(Mode, "Kavenegar", StringComparison.OrdinalIgnoreCase);

    public bool IsCompleteKavenegar()
        => IsKavenegar
           && !string.IsNullOrWhiteSpace(Kavenegar.ApiKey)
           && !string.IsNullOrWhiteSpace(Kavenegar.Sender)
           && !string.IsNullOrWhiteSpace(Kavenegar.OtpTemplate)
           && Uri.TryCreate(Kavenegar.BaseUrl, UriKind.Absolute, out _)
           && Kavenegar.TimeoutSeconds > 0;
}

public sealed class KavenegarSmsOptions
{
    public string BaseUrl { get; set; } = "https://api.kavenegar.com/v1";

    public string Sender { get; set; } = "";

    public string OtpTemplate { get; set; } = "";

    public int TimeoutSeconds { get; set; } = 15;

    public string ApiKey { get; set; } = "";
}
