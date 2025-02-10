using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddGrpc().AddJsonTranscoding();

builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(options =>
// {
//     // Define manual Swagger documentation for your proxy routes
//     options.SwaggerDoc("v1", new OpenApiInfo { Title = "API Gateway", Version = "v1" });
//
//     // Manually add paths for proxy routes
//     options.AddServer(new OpenApiServer(){ Url = "http://localhost:5280" });
// });

builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapReverseProxy();

app.Run();