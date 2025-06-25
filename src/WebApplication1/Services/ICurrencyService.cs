namespace WebApplication1.Others;

public interface ICurrencyService
{
    Task<CurrencyExchangeRate> GetExchangeRateAsync(string targetCurrency);
    Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency);
} 