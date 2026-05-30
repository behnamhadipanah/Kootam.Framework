using System.Net;
using Kootam.Extensions.Authentication.Abstractions.Models;
using Kootam.Extensions.Authentication.Abstractions.Tokens;
using Kootam.Extensions.Authentication.Contracts.Responses;
using Kootam.Extensions.Authentication.Jwt.Options;
using Kootam.Extensions.Authentication.Jwt.Sample.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Kootam.Extensions.Authentication.Jwt.Sample.Controllers;
[Route("api/[controller]")]
[ApiController]
public class JwtController(ITokenGenerator<LogginedUserViewModel> tokenGenerator) : ControllerBase
{
    [HttpGet]
    public IActionResult Test([FromServices] IOptions<JwtOptions> options)
    {
        return Ok(options.Value);
    }

    [HttpPost]
    public async Task<IActionResult> GenerateToken()
    {
        LogginedUserViewModel user = new LogginedUserViewModel()
        {
            Id = 1,
            Email = "test@kootamGroup.com",
            FirstName = "Kootam",
            LastName = "Group"
        };

       string token= tokenGenerator.GenerateAccessToken(user);
       IPHostEntry ip = Dns.GetHostEntry(Dns.GetHostName());

       RefreshToken refreshToken = tokenGenerator.GenerateRefreshToken(ip.HostName.ToString());
       
       return Ok(new ResponseLogin(token, refreshToken));
    }
}