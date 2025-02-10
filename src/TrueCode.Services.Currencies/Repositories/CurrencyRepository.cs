using Dapper;
using Npgsql;
using TrueCode.Services.Currencies.Configuration;
using TrueCode.Services.Currencies.Contracts;
using TrueCode.Services.Currencies.Models;

namespace TrueCode.Services.Currencies.Repositories;

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

    public async Task<IReadOnlyCollection<Currency>> GetFavoriteCurrenciesAsync(int userId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var query = @"
                SELECT c.id, c.name, c.code, c.rate
                FROM currencies c
                INNER JOIN favorites f ON c.id = f.currency_id
                WHERE f.user_id = @UserId";
            
        var results = await connection.QueryAsync<Currency>(query, new { UserId = userId });
            
        return results.ToList().AsReadOnly();
    }
    
    public async Task<bool> AddCurrencyToFavoritesAsync(int userId, int currencyId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var query = @"INSERT INTO favorites (user_id, currency_id) VALUES (@UserId, @CurrencyId) 
                      ON CONFLICT (user_id, currency_id) DO NOTHING";
        
        var rows = await connection.ExecuteAsync(query, new { UserId = userId, CurrencyId = currencyId });
        return rows > 0;
    }
    
    public async Task<bool> RemoveCurrencyFromFavoritesAsync(int userId, int currencyId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var query = "DELETE FROM favorites WHERE user_id = @UserId AND currency_id = @CurrencyId";
        
        var rows = await connection.ExecuteAsync(query, new { UserId = userId, CurrencyId = currencyId });
        return rows > 0;
    }
}