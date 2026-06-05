using FluentValidation;
using Kootam.Cqrs.DependencyInjections;
using Kootam.Cqrs.Sample.Commands.FooCommands.Create;
using Kootam.Cqrs.Sample.Queries.FooQueries;
using Kootam.Utilities.ScalarRegistration.DependencyInjection;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();
builder.Services.AddScalar(options =>
{
    options.Enabled = true;
});

builder.Services.AddCqrs(options =>
{
    options.RegisterServicesFromAssemblyContaining<CreateFooCommandHandler>();
    options.RegisterServicesFromAssemblyContaining<GetFooListQueryHandler>();
    //options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    options.EnableLogging = true;
    options.EnableValidation = true;
});

var app = builder.Build();

app.MapControllers();

app.UseScalar();
app.UseHttpsRedirection();


app.Run();

