namespace Kootam.Email;

public sealed record EmailCard(
    string Subject,
    string? LogoUrl,
    string Body,
    string? Title = null,
    string? Highlight = null,
    string? HighlightHref = null,
    string? Footer = null);

public sealed record EmailDocument(string Subject, string PlainText, string Html);
