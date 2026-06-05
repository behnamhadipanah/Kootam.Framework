namespace Kootam.Authentication.Contracts.Requests;

public record LoginDto(string UserName,string Password,bool RememberMe = false,string ReturnUrl = "/");

