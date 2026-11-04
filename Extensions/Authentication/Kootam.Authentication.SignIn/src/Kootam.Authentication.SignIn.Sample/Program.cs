using Kootam.Authentication.Abstractions.Enums;
using Kootam.Authentication.Abstractions.Services;
using Kootam.Authentication.SignIn.DependencyInjection;
using Kootam.Caching.InMemory.DependencyInjection;
using Kootam.Email;
using Kootam.Email.DependencyInjection;
using Kootam.Sms.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInMemoryCaching();
builder.Services.AddKootamEmail(builder.Configuration);
builder.Services.AddKootamSms(builder.Configuration);
builder.Services.AddKootamSignIn(builder.Configuration);

var app = builder.Build();

app.MapPost("/sign-in/email/request", async (DestinationBody body, IContactOtpService otp) =>
    Results.Ok(await otp.RequestAsync(ContactOtpPurpose.EmailLogin, body.Destination)));

app.MapPost("/sign-in/email/verify", async (VerifyBody body, IContactOtpService otp) =>
    Results.Ok(await otp.VerifyAsync(ContactOtpPurpose.EmailLogin, body.Destination, body.Code)));

app.MapPost("/sign-in/mobile/request", async (DestinationBody body, IContactOtpService otp) =>
    Results.Ok(await otp.RequestAsync(ContactOtpPurpose.MobileLogin, body.Destination)));

app.MapPost("/sign-in/mobile/verify", async (VerifyBody body, IContactOtpService otp) =>
    Results.Ok(await otp.VerifyAsync(ContactOtpPurpose.MobileLogin, body.Destination, body.Code)));

app.MapGet("/email/preview", (IEmailDocumentRenderer renderer, IOptions<EmailOptions> email) =>
{
    var document = renderer.Render(new EmailCard(
        Subject: "Sign-in code",
        LogoUrl: email.Value.LogoUrl,
        Body: "Use this code to continue.",
        Highlight: "123456",
        Footer: "This code expires in 10 minutes."));
    return Results.Content(document.Html, "text/html");
});

app.Run();

internal sealed record DestinationBody(string Destination);

internal sealed record VerifyBody(string Destination, string Code);
