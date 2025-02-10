using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TrueCode.Services.Fetcher.Contracts;
using TrueCode.Services.Fetcher.DTO;
using TrueCode.Services.Fetcher.Models;

namespace TrueCode.Services.Fetcher.Services;

public class CurrencyFetchService : ICurrencyFetchService
{
    private readonly ILogger<CurrencyFetchService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMapper _mapper;
    private readonly string _apiUrl;

    public CurrencyFetchService(
        ILogger<CurrencyFetchService> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IMapper mapper)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _mapper = mapper;
        _apiUrl = configuration.GetValue<string>("API_URL") ?? string.Empty;

        if (string.IsNullOrEmpty(_apiUrl))
        {
            _logger.LogError("API URL is empty.");
        }
    }

    public async Task<IReadOnlyCollection<Currency>> FetchCurrencies(CancellationToken cancellationToken = default)
    {
        using var client = _httpClientFactory.CreateClient();
        
        var response = await client.GetAsync(_apiUrl, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Error fetching currencies.");
            return Array.Empty<Currency>();
        }
        
        var encoding = Encoding.GetEncoding("windows-1251");
        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var content = encoding.GetString(bytes);

        var serializer = new XmlSerializer(typeof(ValCurs));
        ValCurs rates;

        using (var reader = new StringReader(content))
        {
            rates = (ValCurs)serializer.Deserialize(reader)!;
        }

        return _mapper.Map<List<Currency>>(rates.Valutes);
    }
}