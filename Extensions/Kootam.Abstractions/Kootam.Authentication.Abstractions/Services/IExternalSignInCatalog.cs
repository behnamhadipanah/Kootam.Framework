using Kootam.Authentication.SignIn.Models;

namespace Kootam.Authentication.SignIn.Contracts;

public interface IExternalSignInCatalog
{
    IReadOnlyList<ExternalProviderDescription> Describe();
}
