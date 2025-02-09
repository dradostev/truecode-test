using Dapper;
using Npgsql;
using TrueCode.Services.Auth.Configuration;
using TrueCode.Services.Auth.Contracts;
using TrueCode.Services.Auth.Models;

namespace TrueCode.Services.Auth.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;
    
    public UserRepository(IConfiguration configuration)
    {
        var config = configuration.Get<DatabaseConfig>();

        if (config is null)
        {
            throw new NullReferenceException("Environment variables are not set.");
        }
        
        _connectionString =
            $"Server={config.Address}:{config.Port};Database={config.Database};User Id={config.Username};Password={config.Password};";
    }
    
    public async Task<User?> GetUserByNameAsync(string name)
    {
        await using var db = new NpgsqlConnection(_connectionString);
        var user = await db.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM users WHERE name = @Name", new { Name = name });
        return user;
    }

    public async Task AddUserAsync(User user)
    {
        await using var db = new NpgsqlConnection(_connectionString);
        await db.ExecuteAsync("INSERT INTO users (name, password) VALUES (@Name, @Password)", user);
    }
}