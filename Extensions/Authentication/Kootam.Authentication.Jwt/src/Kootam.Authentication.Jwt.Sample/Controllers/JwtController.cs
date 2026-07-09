using System.Net;
using Kootam.Authentication.Abstractions.Models;
using Kootam.Authentication.Abstractions.Tokens;
using Kootam.Authentication.Contracts.Responses;
using Kootam.Authentication.Jwt.Options;
using Kootam.Authentication.Jwt.Sample.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Kootam.Authentication.Jwt.Sample.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JwtController(ITokenGenerator<LoggingUserViewModel> tokenGenerator) : ControllerBase
{
    [HttpGet]
    public IActionResult Test([FromServices] IOptions<JwtOptions> options)
    {
        return Ok(options.Value);
    }

    [HttpPost]
    public async Task<IActionResult> GenerateToken()
    {
        LoggingUserViewModel user = new LoggingUserViewModel()
        {
            Id = 1,
            Email = "test@kootamGroup.com",
            FirstName = "Kootam",
            LastName = "Group"
        };

        IssuedAccessToken accessToken = tokenGenerator.GenerateAccessToken(user);
        IPHostEntry ip = Dns.GetHostEntry(Dns.GetHostName());

        RefreshToken refreshToken = tokenGenerator.GenerateRefreshToken(ip.HostName.ToString());

        return Ok(new IssuedToken()
        {
            AccessToken = accessToken.Token,AccessTokenExpires = accessToken.Expires,
            RefreshToken = refreshToken,RefreshTokenExpires = refreshToken.Expires
        });
    }
}