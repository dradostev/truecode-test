using System.Text;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TrueCode.Services.Currencies.Contracts;
using TrueCode.Services.Currencies.Middlewares;
using TrueCode.Services.Currencies.Repositories;
using TrueCode.Services.Currencies.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ICurrencyRepository, CurrencyRepository>();
var key = builder.Configuration["JwtConfig:SecretKey"];
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // предположим, что у нас и правда есть identity server, все равно это тестовое задание
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSingleton<Interceptor, AuthorizationInterceptor>();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<CurrenciesService>();

app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();