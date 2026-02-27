namespace Kootam.Framework.Authentication.Jwt.Dtos;

public record LoginDto(string UserName, string Password, bool RememberMe = false, string ReturnUrl = "/")
{
}
