using encrypt_server.Repositories;
using encrypt_server.Services;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
var secret = Environment.GetEnvironmentVariable("ENCRYPTION_SECRET");


builder.Services.AddSingleton<EmployeeService>(new EmployeeService(new EmployeeRepository(connectionString!, secret!)));

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();