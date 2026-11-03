using System.Net;
using System.Text;

namespace Kootam.Email;

public sealed class EmailCardRenderer : IEmailDocumentRenderer
{
    public EmailDocument Render(EmailCard card)
    {
        ArgumentNullException.ThrowIfNull(card);
        if (string.IsNullOrWhiteSpace(card.Subject))
            throw new ArgumentException("Email subject is required.", nameof(card));
        if (string.IsNullOrWhiteSpace(card.Body))
            throw new ArgumentException("Email body is required.", nameof(card));

        var title = string.IsNullOrWhiteSpace(card.Title) ? card.Subject.Trim() : card.Title.Trim();
        var logo = LogoHtml(card.LogoUrl);
        var highlight = HighlightHtml(card.Highlight, card.HighlightHref);
        var footer = string.IsNullOrWhiteSpace(card.Footer)
            ? string.Empty
            : $"""<p style="margin:12px 0 0 0;font-size:13px;line-height:1.55;color:#6b6b6b;">{Encode(card.Footer)}</p>""";

        var html = $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
              <meta charset="utf-8">
              <meta name="viewport" content="width=device-width, initial-scale=1">
              <title>{Encode(title)}</title>
            </head>
            <body style="margin:0;padding:0;background-color:#f7f7f8;">
              <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" style="background-color:#f7f7f8;">
                <tr>
                  <td align="center" style="padding:40px 16px;">
                    <table role="presentation" width="560" cellpadding="0" cellspacing="0" border="0" style="width:100%;max-width:560px;background-color:#ffffff;border:1px solid #e6e6e6;border-radius:12px;">
                      <tr>
                        <td style="padding:36px 40px 32px 40px;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Helvetica,Arial,sans-serif;color:#171717;">
                          {logo}
                          <h1 style="margin:28px 0 0 0;font-size:22px;line-height:1.3;font-weight:700;letter-spacing:-0.02em;color:#171717;">{Encode(title)}</h1>
                          <p style="margin:16px 0 0 0;font-size:15px;line-height:1.5;color:#171717;">{Encode(card.Body)}</p>
                          {highlight}
                          <div style="margin:28px 0 0 0;border-top:1px solid #e6e6e6;font-size:0;line-height:0;">&nbsp;</div>
                          {footer}
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </body>
            </html>
            """;

        return new EmailDocument(card.Subject.Trim(), PlainText(title, card), html);
    }

    private static string LogoHtml(string? logoUrl)
    {
        if (!IsSafeLogoUrl(logoUrl))
            return string.Empty;

        return $"""
            <img src="{Encode(logoUrl!.Trim())}" alt="" width="44" height="44" style="display:block;width:44px;height:44px;border:0;border-radius:10px;">
            """;
    }

    private static string HighlightHtml(string? highlight, string? href)
    {
        if (string.IsNullOrWhiteSpace(highlight))
            return string.Empty;

        var encoded = Encode(highlight);
        if (!IsSafeLink(href))
        {
            return $"""<p style="margin:20px 0 0 0;font-size:32px;line-height:1.2;font-weight:700;letter-spacing:0.08em;color:#171717;">{encoded}</p>""";
        }

        return $"""<p style="margin:20px 0 0 0;font-size:15px;line-height:1.5;"><a href="{Encode(href!.Trim())}" style="color:#171717;">{encoded}</a></p>""";
    }

    internal static bool IsSafeLogoUrl(string? logoUrl)
    {
        if (string.IsNullOrWhiteSpace(logoUrl) || !Uri.TryCreate(logoUrl.Trim(), UriKind.Absolute, out var uri))
            return false;

        return uri.Scheme is "https" or "http";
    }

    private static bool IsSafeLink(string? href)
    {
        if (string.IsNullOrWhiteSpace(href) || !Uri.TryCreate(href.Trim(), UriKind.Absolute, out var uri))
            return false;

        return uri.Scheme is "https" or "http" or "mailto";
    }

    private static string PlainText(string title, EmailCard card)
    {
        var text = new StringBuilder();
        text.AppendLine(title);
        text.AppendLine();
        text.AppendLine(card.Body.Trim());
        if (!string.IsNullOrWhiteSpace(card.Highlight))
        {
            text.AppendLine();
            text.AppendLine(card.Highlight.Trim());
            if (IsSafeLink(card.HighlightHref))
            {
                text.AppendLine(card.HighlightHref!.Trim());
            }
        }

        if (!string.IsNullOrWhiteSpace(card.Footer))
        {
            text.AppendLine();
            text.AppendLine(card.Footer.Trim());
        }

        return text.ToString();
    }

    private static string Encode(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);
}
