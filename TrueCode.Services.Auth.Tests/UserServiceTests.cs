using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrueCode.Services.Auth.Configuration;
using TrueCode.Services.Auth.Contracts;
using TrueCode.Services.Auth.Models;
using TrueCode.Services.Auth.Services;

namespace TrueCode.Services.Auth.Tests;

using Moq;
using Xunit;
using System.Threading.Tasks;
using Grpc.Core;

public class UserServiceTests
{
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<IOptions<JwtConfig>> _mockJwtConfig;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockLogger = new Mock<ILogger<UserService>>();
        _mockRepository = new Mock<IUserRepository>();

        _mockJwtConfig = new Mock<IOptions<JwtConfig>>();
        _mockJwtConfig.Setup(config => config.Value).Returns(new JwtConfig
        {
            AccessTokenExpiresMins = 30,
            RefreshTokenExpiresDays = 7,
            SecretKey = "rgy842iuptjs5ctfirpksi0eq7e6vrq6q4u8z9adnf4e4vdllg74zoqdkms7624nubcrjcr7v9jtim74oqso2mz2jpo3hgpkv8grgxvfd23dme1llgdl9o2en387iev2"
        });

        _userService = new UserService(
            _mockLogger.Object,
            _mockRepository.Object,
            _mockJwtConfig.Object
        );
    }

    [Fact]
    public async Task Register_UserAlreadyExists_ThrowsRpcException()
    {
        var request = new RegisterRequest { Name = "auntiesraka", Password = "p@assw0rd256" };
        _mockRepository
            .Setup(repo => repo.GetUserByNameAsync(request.Name))
            .ReturnsAsync(new User ("auntiesraka", BCrypt.Net.BCrypt.HashPassword("p@assw0rd256") ));
        
        var exception = await Assert.ThrowsAsync<RpcException>(() => _userService.Register(request, null));
        
        Assert.Equal(StatusCode.AlreadyExists, exception.StatusCode);
    }

    [Fact]
    public async Task Register_Success_ReturnsRegisterResponse()
    {
        var request = new RegisterRequest { Name = "bolzhedor", Password = "p@assw0rd256" };
        
        _mockRepository
            .Setup(repo => repo.GetUserByNameAsync(request.Name))
            .ReturnsAsync((User?)null);
        _mockRepository
            .Setup(repo => repo.AddUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var response = await _userService.Register(request, null);

        Assert.Equal("Registration has been completed successfully.", response.Message);
    }

    [Fact]
    public async Task Authenticate_UserNotFound_ThrowsRpcException()
    {
        var request = new AuthRequest { Name = "bolzhedor", Password = "p@assw0rd256" };
        
        _mockRepository
            .Setup(repo => repo.GetUserByNameAsync(request.Name))
            .ReturnsAsync((User)null);
        
        var exception = await Assert.ThrowsAsync<RpcException>(() => _userService.Authenticate(request, null));
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task Authenticate_InvalidPassword_ThrowsRpcException()
    {
        var request = new AuthRequest { Name = "auntiesraka", Password = "wrongPassword" };
        var user = new User("auntiesraka", "$2b$10$17/1zU40vI1JI69jUmwVfeJErqCqOKm4QZ5sMyLO7DID4.lRuwkya");
        
        _mockRepository
            .Setup(repo => repo.GetUserByNameAsync(request.Name))
            .ReturnsAsync(user);

        var exception = await Assert.ThrowsAnyAsync<RpcException>(() => _userService.Authenticate(request, null));
        Assert.Equal(StatusCode.Unauthenticated, exception.StatusCode);
    }

    [Fact]
    public async Task Authenticate_Success_ReturnsAuthResponse()
    {
        var request = new AuthRequest { Name = "auntiesraka", Password = "123456" };
        var user = new User("auntiesraka", "$2b$10$17/1zU40vI1JI69jUmwVfeJErqCqOKm4QZ5sMyLO7DID4.lRuwkya");
        
        _mockRepository
            .Setup(repo => repo.GetUserByNameAsync(request.Name))
            .ReturnsAsync(user);
        _mockRepository
            .Setup(repo => repo.AddUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var response = await _userService.Authenticate(request, null);

        Assert.NotNull(response.AccessToken);
        Assert.NotNull(response.RefreshToken);
    }
}
