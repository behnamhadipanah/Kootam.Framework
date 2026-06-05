using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Extensions.Authentication.Builder;

public class AuthenticationBuilder
{
    public IServiceCollection Services { get; }

    public AuthenticationBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
