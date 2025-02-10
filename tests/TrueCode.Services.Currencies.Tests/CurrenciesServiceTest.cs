using CurrencyService;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using TrueCode.Services.Currencies.Contracts;
using TrueCode.Services.Currencies.Models;
using TrueCode.Services.Currencies.Services;

namespace TrueCode.Services.Currencies.Tests;

public class CurrenciesServiceTests
{
    private readonly Mock<ICurrencyRepository> _currencyRepositoryMock;
    private readonly CurrenciesService _service;

    public CurrenciesServiceTests()
    {
        _currencyRepositoryMock = new Mock<ICurrencyRepository>();
        var loggerMock = new Mock<ILogger<CurrenciesService>>();
        _service = new CurrenciesService(_currencyRepositoryMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task GetFavorites_ShouldReturnListOfCurrencies()
    {
        var userId = 5;
        var currencies = new List<Currency>
        {
            new() { Id = 1, Name = "Российский рубль", Code = "RUB", Rate = 1.0M },
            new() { Id = 2, Name = "Доллар США", Code = "USD", Rate = 92.5M },
            new() { Id = 3, Name = "Евро", Code = "EUR", Rate = 100.7M }
        };
        
        _currencyRepositoryMock
            .Setup(repo => repo.GetFavoriteCurrenciesAsync(userId))
            .ReturnsAsync(currencies);
        
        var request = new UserRequest { UserId = userId };
        var context = new Mock<ServerCallContext>();

        var response = await _service.GetFavorites(request, context.Object);

        Assert.Equal(3, response.Currencies.Count);
        Assert.Equal("RUB", response.Currencies[0].Code);
        Assert.Equal("USD", response.Currencies[1].Code);
        Assert.Equal("EUR", response.Currencies[2].Code);
    }

    [Fact]
    public async Task AddFavorite_ShouldReturnSuccessMessage()
    {
        var request = new AddFavoriteRequest { UserId = 5, CurrencyId = 1 };
        var context = new Mock<ServerCallContext>();
        
        _currencyRepositoryMock
            .Setup(repo => repo.AddCurrencyToFavoritesAsync(request.UserId, request.CurrencyId))
            .Returns(Task.FromResult(true));

        var response = await _service.AddFavorite(request, context.Object);

        Assert.Equal("Currency added to favorites.", response.Message);
    }

    [Fact]
    public async Task RemoveFavorite_ShouldReturnSuccessMessage_WhenCurrencyExists()
    {
        var request = new RemoveFavoriteRequest { UserId = 5, CurrencyId = 1 };
        var context = new Mock<ServerCallContext>();
        
        _currencyRepositoryMock
            .Setup(repo => repo.RemoveCurrencyFromFavoritesAsync(request.UserId, request.CurrencyId))
            .ReturnsAsync(true);

        var response = await _service.RemoveFavorite(request, context.Object);

        Assert.Equal("Currency removed from favorites.", response.Message);
    }

    [Fact]
    public async Task RemoveFavorite_ShouldThrowRpcException_WhenCurrencyDoesNotExist()
    {
        var request = new RemoveFavoriteRequest { UserId = 5, CurrencyId = 1 };
        var context = new Mock<ServerCallContext>();
        
        _currencyRepositoryMock
            .Setup(repo => repo.RemoveCurrencyFromFavoritesAsync(request.UserId, request.CurrencyId))
            .ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<RpcException>(() => _service.RemoveFavorite(request, context.Object));
        
        Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
        Assert.Equal("Currency not found in favorites.", exception.Status.Detail);
    }
}
