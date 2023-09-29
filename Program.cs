using encrypt_server.Repositories;
using encrypt_server.Services;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
var secret = Environment.GetEnvironmentVariable("ENCRYPTION_SECRET");


builder.Services.AddSingleton<EmployeeService>(new EmployeeService(new EmployeeRepository(connectionString!, secret!)));

var app = builder.Build();

app.Use(async (context, next) =>
{
    var apiKey = Environment.GetEnvironmentVariable("API_KEY");

    if (!context.Request.Headers.TryGetValue("x-api-key", out var requestApiKey))
    {
        context.Response.StatusCode = 401; 
        await context.Response.WriteAsync("API key is missing.");
        return;
    }

    if (!string.Equals(requestApiKey, apiKey, StringComparison.OrdinalIgnoreCase))
    {
        context.Response.StatusCode = 401; 
        await context.Response.WriteAsync("Invalid API key.");
        return;
    }
    await next(context);
});

app.UseAuthorization();

app.MapControllers();

app.Run();