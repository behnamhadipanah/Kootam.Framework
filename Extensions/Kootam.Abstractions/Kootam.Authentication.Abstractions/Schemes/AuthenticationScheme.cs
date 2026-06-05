namespace Kootam.Authentication.Abstractions.Schemes;

public class AuthenticationScheme
{
    public string Name { get; set; }
    public Type HandlerType { get; set; }
}
