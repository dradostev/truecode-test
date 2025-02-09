using TrueCode.Services.Auth.Models;

namespace TrueCode.Services.Auth.Contracts;

public interface IUserRepository
{
    Task<User?> GetUserByNameAsync(string name);
    Task AddUserAsync(User user);
}