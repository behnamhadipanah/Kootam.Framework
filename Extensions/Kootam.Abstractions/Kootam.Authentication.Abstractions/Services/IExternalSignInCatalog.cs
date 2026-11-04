using Kootam.Authentication.Abstractions.Models;

namespace Kootam.Authentication.Abstractions.Services;

public interface IExternalSignInCatalog
{
    IReadOnlyList<ExternalProviderDescription> Describe();
}
