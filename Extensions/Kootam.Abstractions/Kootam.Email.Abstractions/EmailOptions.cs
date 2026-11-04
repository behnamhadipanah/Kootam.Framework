namespace Kootam.Email;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    public string Mode { get; set; } = "Log";

    public string? LogoUrl { get; set; }

    public string? FromName { get; set; }

    public string? FromAddress { get; set; }

    public SmtpEmailOptions Smtp { get; set; } = new();

    public bool IsLog => string.Equals(Mode, "Log", StringComparison.OrdinalIgnoreCase);

    public bool IsSmtp => string.Equals(Mode, "Smtp", StringComparison.OrdinalIgnoreCase);

    public bool SmtpIsReady()
        => !string.IsNullOrWhiteSpace(Smtp.Host)
           && !string.IsNullOrWhiteSpace(FromAddress)
           && !string.IsNullOrWhiteSpace(Smtp.User)
           && !string.IsNullOrWhiteSpace(Smtp.Password)
           && Smtp.Port > 0;
}

public sealed class SmtpEmailOptions
{
    public const int DefaultPort = 587;

    public const int DefaultImplicitSslPort = 465;

    public string? Host { get; set; }

    public string? ConnectHost { get; set; }

    public int Port { get; set; } = DefaultPort;

    public int ImplicitSslPort { get; set; } = DefaultImplicitSslPort;

    public bool UseSsl { get; set; } = true;

    public string? User { get; set; }

    public string? Password { get; set; }

    public int TimeoutSeconds { get; set; } = 15;

    public bool UsesImplicitSsl()
        => ImplicitSslPort > 0 && Port == ImplicitSslPort;
}
