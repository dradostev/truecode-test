using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using TrueCode.Services.Fetcher.Configuration;
using TrueCode.Services.Fetcher.Contracts;
using TrueCode.Services.Fetcher.Models;

namespace TrueCode.Services.Fetcher.Repositories;

public class CurrencyRepository : ICurrencyRepository
{
    private readonly string _connectionString;

    public CurrencyRepository(IConfiguration configuration)
    {
        var config = configuration.Get<DatabaseConfig>();

        if (config is null)
        {
            throw new NullReferenceException("Environment variables are not set.");
        }

        _connectionString =
            $"Server={config.Address}:{config.Port};Database={config.Database};User Id={config.Username};Password={config.Password};";
    }
    
    public async Task UpdateAsync(IReadOnlyCollection<Currency> currencies, CancellationToken cancellationToken = default)
    {
        const string query = @"
            INSERT INTO currencies (code, name, rate)
            VALUES (@Code, @Name, @Rate)
            ON CONFLICT (code) DO UPDATE
            SET name = EXCLUDED.name,
                rate = EXCLUDED.rate;";

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        await connection.ExecuteAsync(query, currencies, transaction);
        await transaction.CommitAsync(cancellationToken);
    }
}