using Kootam.Caching.Redis.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

/*
 builder.Services.AddRedisCaching(options =>
{
    options.Configs = new List<RedisDBConfigModel>
    {
        new RedisDBConfigModel
        {
            Name = "Default",
            Host = "localhost",
            Port = 6379,
            DBNumber = 0
        }
    };
});
 */
 /*
  builder.Services.AddRedisCaching(
    host: "localhost",
    port: 6379,
    database: 0,
    password: null,
    configName: "Default");
  */
#region activate healthChecks
builder.Services
    .AddRedisCaching(builder.Configuration)
    .AddRedisHealthChecks();

#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapHealthChecks("/health");

app.Run();

