using TrueCode.Services.Auth.Configuration;
using TrueCode.Services.Auth.Contracts;
using TrueCode.Services.Auth.Repositories;
using TrueCode.Services.Auth.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<UserService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();