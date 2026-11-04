using Kootam.Email;

namespace Kootam.Email.UnitTest;

public class SmtpEmailOptionsTests
{
    [Fact]
    public void Default_port_uses_starttls_and_keeps_implicit_ssl_on_465()
    {
        var smtp = new SmtpEmailOptions();

        Assert.Equal(SmtpEmailOptions.DefaultPort, smtp.Port);
        Assert.Equal(SmtpEmailOptions.DefaultImplicitSslPort, smtp.ImplicitSslPort);
        Assert.False(smtp.UsesImplicitSsl());

        smtp.Port = SmtpEmailOptions.DefaultImplicitSslPort;
        Assert.True(smtp.UsesImplicitSsl());
    }

    [Fact]
    public void Implicit_ssl_follows_a_custom_port()
    {
        var smtp = new SmtpEmailOptions
        {
            Port = 8465,
            ImplicitSslPort = 8465
        };

        Assert.True(smtp.UsesImplicitSsl());
    }

    [Fact]
    public void Implicit_ssl_can_be_disabled()
    {
        var smtp = new SmtpEmailOptions
        {
            Port = 465,
            ImplicitSslPort = 0
        };

        Assert.False(smtp.UsesImplicitSsl());
    }
}
