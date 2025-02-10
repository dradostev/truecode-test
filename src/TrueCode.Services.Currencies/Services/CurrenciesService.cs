using CurrencyService;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using TrueCode.Services.Currencies.Contracts;

namespace TrueCode.Services.Currencies.Services;

[Authorize]
public class CurrenciesService : CurrencyService.CurrencyService.CurrencyServiceBase
{
    private readonly ICurrencyRepository _currencyRepository;

    public CurrenciesService(ICurrencyRepository currencyRepository, ILogger<CurrenciesService> logger)
    {
        _currencyRepository = currencyRepository;
    }

    public override async Task<CurrencyListResponse> GetFavorites(UserRequest request, ServerCallContext context)
    {
        var currencies = await _currencyRepository.GetFavoriteCurrenciesAsync(request.UserId);
        var response = new CurrencyListResponse();

        foreach (var currency in currencies)
        {
            response.Currencies.Add(new CurrencyInfo
            {
                Id = currency.Id,
                Name = currency.Name,
                Code = currency.Code,
                Rate = (double)currency.Rate
            });
        }

        return response;
    }
    
    public override async Task<AddFavoriteResponse> AddFavorite(AddFavoriteRequest request, ServerCallContext context)
    {
        await _currencyRepository.AddCurrencyToFavoritesAsync(request.UserId, request.CurrencyId);
        
        return new AddFavoriteResponse
        {
            Message = "Currency added to favorites."
        };
    }
    
    public override async Task<RemoveFavoriteResponse> RemoveFavorite(RemoveFavoriteRequest request, ServerCallContext context)
    {
        var success = await _currencyRepository.RemoveCurrencyFromFavoritesAsync(request.UserId, request.CurrencyId);

        if (!success)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Currency not found in favorites."));
        }
        
        return new RemoveFavoriteResponse
        {
            Message = "Currency removed from favorites."
        };
    }
}