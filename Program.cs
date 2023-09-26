using encrypt_server.Repositories;
using encrypt_server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<AuthService>(new AuthService(new UserRepository("Host=localhost;Username=app_user;Password=12345;Database=secrets")));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();