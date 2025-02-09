using TrueCode.Services.Fetcher.Models;

namespace TrueCode.Services.Fetcher.Contracts;

public interface ICurrencyRepository
{
    Task UpdateAsync(IReadOnlyCollection<Currency> currencies, CancellationToken cancellationToken = default);
}