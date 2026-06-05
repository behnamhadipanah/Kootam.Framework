using Kootam.Extensions.Authentication.Abstractions.Claims;
using Kootam.Extensions.Authentication.DependencyInjection;
using Kootam.Extensions.Authentication.Jwt.DependencyInjection;
using Kootam.Extensions.Authentication.Jwt.Sample.Mappers;
using Kootam.Extensions.Authentication.Jwt.Sample.ViewModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddScoped<IUserClaimsMapper<LogginedUserViewModel>, LogginedUserClaimsMapper>();


builder.Services
    .AddKootamAuthentication("SampleScheme")
    .AddJwt<LogginedUserViewModel>(builder.Configuration);


/*
 builder.Services
    .AddKootamAuthentication("SampleScheme")
    .AddJwt<LogginedUserViewModel>(options =>
    {
        options.Key = "secret";
        options.Issuer = "Kootam";
        options.Audience = "KootamApi";
    });
 */
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
