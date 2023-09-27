using encrypt_server.Repositories;
using encrypt_server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = "Host=localhost;Username=admin;Password=12345;Database=business_crud";

builder.Services.AddSingleton<BusinessTypeService>(new BusinessTypeService(new BusinessTypeRepository(connectionString)));

builder.Services.AddSingleton<StateService>(new StateService(new StateRepository(connectionString)));

builder.Services.AddSingleton<AuthService>(new AuthService(new UserRepository(connectionString)));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();