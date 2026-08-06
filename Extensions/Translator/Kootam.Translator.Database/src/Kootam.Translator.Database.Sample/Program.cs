using Kootam.Translator.Abstractions;
using Kootam.Translator.Database.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.AddDbTranslator()
    .UseCaching(reloadIntervalInMinutes: 5);
// builder.AddDbTranslator().WithoutCaching();
// builder.AddDbTranslator().UseMigrations();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseTranslator();

app.MapGet("/", (ITranslator translator) => translator["WelcomeMessage"]);

app.Run();
