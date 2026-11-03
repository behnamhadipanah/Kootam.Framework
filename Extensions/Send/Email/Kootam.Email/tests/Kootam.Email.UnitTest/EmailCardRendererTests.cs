using Kootam.Email;

namespace Kootam.Email.UnitTest;

public class EmailCardRendererTests
{
    private readonly EmailCardRenderer renderer = new();

    [Fact]
    public void Render_includes_logo_subject_body_and_code()
    {
        var document = renderer.Render(new EmailCard(
            Subject: "Sign-in code",
            LogoUrl: "https://cdn.example/logo.png",
            Body: "Use this code to continue.",
            Highlight: "123456",
            Footer: "This code expires in 10 minutes."));

        Assert.Equal("Sign-in code", document.Subject);
        Assert.Contains("https://cdn.example/logo.png", document.Html, StringComparison.Ordinal);
        Assert.Contains("Use this code to continue.", document.Html, StringComparison.Ordinal);
        Assert.Contains("123456", document.Html, StringComparison.Ordinal);
        Assert.Contains("This code expires in 10 minutes.", document.PlainText, StringComparison.Ordinal);
    }

    [Fact]
    public void Render_encodes_markup()
    {
        var document = renderer.Render(new EmailCard(
            Subject: "Subject",
            LogoUrl: "javascript:alert(1)",
            Body: "<script>alert(1)</script>",
            Highlight: "<b>code</b>"));

        Assert.DoesNotContain("<script>", document.Html, StringComparison.Ordinal);
        Assert.DoesNotContain("javascript:alert", document.Html, StringComparison.Ordinal);
        Assert.Contains("&lt;script&gt;", document.Html, StringComparison.Ordinal);
        Assert.Contains("&lt;b&gt;code&lt;/b&gt;", document.Html, StringComparison.Ordinal);
    }
}
