using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using TrueCode.Services.Migrations.Configuration;

using var serviceProvider = ConfigProvider.Create;
using var scope = serviceProvider.CreateScope();

var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
runner.MigrateUp();