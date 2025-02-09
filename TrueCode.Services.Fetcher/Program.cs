using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TrueCode.Services.Fetcher.Contracts;
using TrueCode.Services.Fetcher.Mapping;
using TrueCode.Services.Fetcher.Repositories;
using TrueCode.Services.Fetcher.Services;
using TrueCode.Services.Fetcher.Workers;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var host = new HostBuilder()
    .ConfigureAppConfiguration((_, config) =>
    {
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((_, services) =>
    {
        services.AddHttpClient();
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddTransient<ICurrencyRepository, CurrencyRepository>();
        services.AddTransient<ICurrencyFetchService, CurrencyFetchService>();
        services.AddHostedService<CurrencyFetchWorker>();
    })
    .ConfigureLogging(logging =>
    {
        logging.AddConsole();
    })
    .Build();

await host.RunAsync();