using Microsoft.Extensions.Diagnostics.HealthChecks;
using TrueCode.Services.Auth.Configuration;
using TrueCode.Services.Auth.Contracts;
using TrueCode.Services.Auth.Repositories;
using TrueCode.Services.Auth.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddGrpc();
builder.Services.AddGrpcHealthChecks().AddCheck("health", () => HealthCheckResult.Healthy());

var app = builder.Build();

app.MapGrpcService<UserService>();
app.MapGrpcHealthChecksService();

app.MapGet("/", () => "auth service");

app.Run();