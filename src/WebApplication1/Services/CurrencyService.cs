using System.Text.Json;

namespace WebApplication1.Others;

//done with the use of internet to correctly get the exchange rates
public class CurrencyService : ICurrencyService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CurrencyService> _logger;
    private readonly Dictionary<string, (decimal Rate, DateTime LastUpdated)> _rateCache = new();
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(15); 

    public CurrencyService(HttpClient httpClient, ILogger<CurrencyService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CurrencyExchangeRate> GetExchangeRateAsync(string targetCurrency)
    {
        if (string.IsNullOrWhiteSpace(targetCurrency) || targetCurrency.ToUpper() == "PLN")
        {
            return new CurrencyExchangeRate
            {
                BaseCurrency = "PLN",
                TargetCurrency = "PLN",
                Rate = 1.0m,
                LastUpdated = DateTime.UtcNow,
                Source = "Internal"
            };
        }
        if (_rateCache.TryGetValue(targetCurrency.ToUpper(), out var cached))
        {
            if (DateTime.UtcNow - cached.LastUpdated < _cacheExpiration)
            {
                return new CurrencyExchangeRate
                {
                    BaseCurrency = "PLN",
                    TargetCurrency = targetCurrency.ToUpper(),
                    Rate = cached.Rate,
                    LastUpdated = cached.LastUpdated,
                    Source = "Cache"
                };
            }
        }

        try
        {
            var url = $"https://api.exchangerate-api.com/v4/latest/PLN";
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ExchangeRateApiResponse>(json);

            if (result?.Rates != null && result.Rates.TryGetValue(targetCurrency.ToUpper(), out var rate))
            {
                var exchangeRate = new CurrencyExchangeRate
                {
                    BaseCurrency = "PLN",
                    TargetCurrency = targetCurrency.ToUpper(),
                    Rate = rate,
                    LastUpdated = DateTime.UtcNow,
                    Source = "exchangerate-api.com"
                };
                
                _rateCache[targetCurrency.ToUpper()] = (rate, DateTime.UtcNow);

                return exchangeRate;
            }
            else
            {
                _logger.LogWarning("Currency {Currency} not found in primary API response", targetCurrency);
                return GetFallbackRate(targetCurrency);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Primary API failed, using fallback rates for {Currency}", targetCurrency);
            return GetFallbackRate(targetCurrency);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching exchange rate for {Currency}", targetCurrency);
            if (_rateCache.TryGetValue(targetCurrency.ToUpper(), out var fallback))
            {
                _logger.LogWarning("Using cached exchange rate for {Currency}", targetCurrency);
                return new CurrencyExchangeRate
                {
                    BaseCurrency = "PLN",
                    TargetCurrency = targetCurrency.ToUpper(),
                    Rate = fallback.Rate,
                    LastUpdated = fallback.LastUpdated,
                    Source = "Cache (Fallback)"
                };
            }
            
            throw new InvalidOperationException($"Unable to fetch exchange rate for {targetCurrency}", ex);
        }
    }

    public async Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency)
    {
        if (fromCurrency.ToUpper() == toCurrency.ToUpper())
        {
            return amount;
        }

        if (fromCurrency.ToUpper() == "PLN")
        {
            var rate = await GetExchangeRateAsync(toCurrency);
            return amount * rate.Rate;
        }
        else if (toCurrency.ToUpper() == "PLN")
        {
            var rate = await GetExchangeRateAsync(fromCurrency);
            return amount / rate.Rate;
        }
        else
        {
            var targetRate = await GetExchangeRateAsync(toCurrency);
            var sourceRate = await GetExchangeRateAsync(fromCurrency);
            return amount * (targetRate.Rate / sourceRate.Rate);
        }
    }

    private CurrencyExchangeRate GetFallbackRate(string targetCurrency)
    {
        var fallbackRates = new Dictionary<string, decimal>
        {
            { "USD", 0.24m },  // 1 PLN ≈ 0.24 USD
            { "EUR", 0.22m },  // 1 PLN ≈ 0.22 EUR
            { "GBP", 0.19m },  // 1 PLN ≈ 0.19 GBP
            { "CHF", 0.22m },  // 1 PLN ≈ 0.22 CHF
            { "CZK", 5.50m },  // 1 PLN ≈ 5.50 CZK
            { "SEK", 2.60m },  // 1 PLN ≈ 2.60 SEK
            { "NOK", 2.70m },  // 1 PLN ≈ 2.70 NOK
            { "DKK", 1.65m },  // 1 PLN ≈ 1.65 DKK
            { "HUF", 90.0m },  // 1 PLN ≈ 90 HUF
            { "RON", 1.10m },  // 1 PLN ≈ 1.10 RON
            { "BGN", 0.43m },  // 1 PLN ≈ 0.43 BGN
            { "HRK", 1.65m },  // 1 PLN ≈ 1.65 HRK
            { "CAD", 0.33m },  // 1 PLN ≈ 0.33 CAD
            { "AUD", 0.37m },  // 1 PLN ≈ 0.37 AUD
            { "JPY", 36.0m },  // 1 PLN ≈ 36 JPY
        };

        if (fallbackRates.TryGetValue(targetCurrency.ToUpper(), out var rate))
        {
            var exchangeRate = new CurrencyExchangeRate
            {
                BaseCurrency = "PLN",
                TargetCurrency = targetCurrency.ToUpper(),
                Rate = rate,
                LastUpdated = DateTime.UtcNow,
                Source = "Fallback (Approximate)"
            };
            
    
            _rateCache[targetCurrency.ToUpper()] = (rate, DateTime.UtcNow);
            
            _logger.LogInformation("Using fallback exchange rate for {Currency}: {Rate}", targetCurrency, rate);
            return exchangeRate;
        }
        
        _logger.LogError("Currency {Currency} not supported in fallback rates", targetCurrency);
        throw new InvalidOperationException($"Currency {targetCurrency} not supported");
    }

    private class ExchangeRateApiResponse
    {
        public string Base { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; } = new();
    }
} 