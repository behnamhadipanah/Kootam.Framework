using Kootam.Authentication.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKootamAuthentication("KootamScheme");
builder.Services.AddCurrentUser();

builder.Services.AddOpenApi();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
