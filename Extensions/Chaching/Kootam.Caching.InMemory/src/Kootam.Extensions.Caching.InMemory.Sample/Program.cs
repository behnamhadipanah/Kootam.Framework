using Kootam.Extensions.Caching.InMemory.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInMemoryCaching();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
