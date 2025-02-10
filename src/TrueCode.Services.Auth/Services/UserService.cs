using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TrueCode.Services.Auth.Configuration;
using TrueCode.Services.Auth.Contracts;
using TrueCode.Services.Auth.Models;

namespace TrueCode.Services.Auth.Services;

public class UserService(
    ILogger<UserService> logger,
    IUserRepository repository,
    IOptions<JwtConfig> jwtConfig) : AuthService.AuthServiceBase
{
    private readonly JwtConfig _jwtConfig = jwtConfig.Value;
    
    public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        var oldUser = await repository.GetUserByNameAsync(request.Name);

        if (oldUser != null)
        {
            logger.LogInformation($"User {oldUser.Name} already exists");
            throw new RpcException(new Status(StatusCode.AlreadyExists, "User already exists."));
        }

        var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User(request.Name, hash);

        await repository.AddUserAsync(user);
        
        logger.LogInformation($"User {user.Name} has been registered.");

        return new RegisterResponse { Message = "Registration has been completed successfully." };
    }
    
    public override async Task<AuthResponse> Authenticate(AuthRequest request, ServerCallContext context)
    {
        var user = await repository.GetUserByNameAsync(request.Name);

        if (user is null)
        {
            logger.LogInformation($"User {request.Name} does not exist.");
            throw new RpcException(new Status(StatusCode.NotFound, "User does not exist."));
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            logger.LogInformation($"User {user.Name} tried to authenticate with wrong password.");
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Wrong password."));
        }

        var access = GenerateToken(user, TimeSpan.FromMinutes(_jwtConfig.AccessTokenExpiresMins));
        var refresh = GenerateToken(user, TimeSpan.FromDays(_jwtConfig.RefreshTokenExpiresDays));
        
        logger.LogInformation($"User {user.Name} has been authenticated.");

        return new AuthResponse
        {
            AccessToken = access,
            RefreshToken = refresh
        };
    }
    
    private string GenerateToken(User user, TimeSpan expires)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfig.SecretKey);

        var now = DateTime.UtcNow;
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name)
            ]),
            Expires = now.Add(expires),
            NotBefore = now,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = handler.CreateToken(descriptor);
        
        return handler.WriteToken(token);
    }
}