namespace Kootam.Email;

public interface IEmailDocumentRenderer
{
    EmailDocument Render(EmailCard card);
}
