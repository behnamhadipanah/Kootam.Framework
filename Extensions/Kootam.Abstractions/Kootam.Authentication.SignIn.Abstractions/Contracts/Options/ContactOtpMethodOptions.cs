namespace Kootam.Authentication.SignIn.Contracts.Options;

public sealed class ContactOtpMethodOptions
{
    public bool Enabled { get; set; } = true;

    public int CodeExpirationMinutes { get; set; } = 10;

    public string Subject { get; set; } = "Sign-in code";

    public string Body { get; set; } = "Use this code to continue.";

    public string Footer { get; set; } = "If you did not request this code, you can ignore this message.";
}
