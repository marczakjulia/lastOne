using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Others;

public class CurrencyExchangeRate
{
    public string BaseCurrency { get; set; } = "PLN";
    public  required string TargetCurrency { get; set; }
    public decimal Rate { get; set; }
    public DateTime LastUpdated { get; set; }
    public required string Source { get; set; }
} 