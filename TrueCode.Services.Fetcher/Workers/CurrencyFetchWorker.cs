using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TrueCode.Services.Fetcher.Contracts;

namespace TrueCode.Services.Fetcher.Workers;

public class CurrencyFetchWorker(
    ILogger<CurrencyFetchWorker> logger,
    ICurrencyFetchService fetchService,
    ICurrencyRepository repository,
    IConfiguration configuration)
    : BackgroundService
{
    private readonly ILogger<CurrencyFetchWorker> _logger = logger;
    private readonly ICurrencyFetchService _fetchService = fetchService;
    private readonly ICurrencyRepository _repository = repository;
    private readonly int _interval = configuration.GetValue<int>("FETCH_INTERVAL_MINS");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(_interval));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation("Starting to fetch currencies...");
            
                var currencies = await _fetchService.FetchCurrencies(stoppingToken);
                await _repository.UpdateAsync(currencies, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Fetching currencies was cancelled.");
        }
        
        _logger.LogInformation("Fetching currencies completed.");
    }
}