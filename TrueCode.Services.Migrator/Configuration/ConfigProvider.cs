using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrueCode.Services.Migrations.Migrations;

namespace TrueCode.Services.Migrations.Configuration;

public static class ConfigProvider
{
    private static IConfiguration Configuration => new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();

    public static ServiceProvider Create
    {
        get
        {
            var config = Configuration.Get<DatabaseConfig>();

            if (config is null)
            {
                throw new NullReferenceException("Environment variables are not set.");
            }
            
            var connectionString =
                $"Server={config.Address}:{config.Port};Database={config.Database};User Id={config.Username};Password={config.Password};";
            
            var services = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(CreateTableUsers).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);

            return services;
        }
    }
}