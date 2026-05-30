using Kootam.Utilities.ScalarRegistration.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddScalar();
//builder.Services.AddScalar(builder.Configuration, "Scalar");
builder.Services.AddScalar(options =>
{
    options.Enabled = true;
    options.Description = "The sample project for how to used this class library";
    options.Name = "Sample Project";
    options.Version = "1.0.0";

});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();

