using TrueCode.Services.Currencies.Models;

namespace TrueCode.Services.Currencies.Contracts;

public interface ICurrencyRepository
{
    Task<IReadOnlyCollection<Currency>> GetFavoriteCurrenciesAsync(int userId);
    Task<bool> AddCurrencyToFavoritesAsync(int userId, int currencyId);
    Task<bool> RemoveCurrencyFromFavoritesAsync(int userId, int currencyId);
}