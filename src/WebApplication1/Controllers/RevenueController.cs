using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Others;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/revenue")]
[Authorize] 
public class RevenueController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrencyService _currencyService;

    public RevenueController(ApplicationDbContext context, ICurrencyService currencyService)
    {
        _context = context;
        _currencyService = currencyService;
    }

    [HttpGet("current")]
    [Authorize(Roles = "Standard,Admin")] 
    public async Task<ActionResult<decimal>> GetCurrentRevenue(int? softwareSystemId = null, string currency = "PLN")
    {
        var signedQuery = _context.Contract.Where(c => c.Status == ContractStatus.Signed);
        
        var createdPaymentsQuery = _context.Payment
            .Where(p => p.Status == PaymentStatus.Completed && p.Contract.Status == ContractStatus.Created);
        
        if (softwareSystemId.HasValue)
        {
            signedQuery = signedQuery.Where(c => c.SoftwareSystemId == softwareSystemId.Value);
            createdPaymentsQuery = createdPaymentsQuery.Where(p => p.Contract.SoftwareSystemId == softwareSystemId.Value);
        }
        
        var signedContractsRevenue = await signedQuery.SumAsync(c => c.FinalPrice);
        var createdContractsRevenue = await createdPaymentsQuery.SumAsync(p => p.Amount);
        var totalRevenueInPLN = signedContractsRevenue + createdContractsRevenue;
        
        if (currency.ToUpper() != "PLN")
        {
            var exchangeRate = await _currencyService.GetExchangeRateAsync(currency.ToUpper());
            var convertedRevenue = totalRevenueInPLN * exchangeRate.Rate;
            return Ok(Math.Round(convertedRevenue, 2));
        }
        
        return Ok(Math.Round(totalRevenueInPLN, 2));
    }
    
    [HttpGet("predicted")]
    [Authorize(Roles = "Standard,Admin")] 
    public async Task<ActionResult<decimal>> GetPredictedRevenue(int? softwareSystemId = null, string currency = "PLN")
    {
        var query = _context.Contract.Where(c => c.Status == ContractStatus.Created || c.Status == ContractStatus.Signed);
        
        if (softwareSystemId.HasValue)
        {
            query = query.Where(c => c.SoftwareSystemId == softwareSystemId.Value);
        }
        var predictedRevenueInPLN = await query.SumAsync(c => c.FinalPrice);
        if (currency.ToUpper() != "PLN")
        {
            var exchangeRate = await _currencyService.GetExchangeRateAsync(currency.ToUpper());
            var convertedRevenue = predictedRevenueInPLN * exchangeRate.Rate;
            return Ok(Math.Round(convertedRevenue, 2));
        }
        
        return Ok(Math.Round(predictedRevenueInPLN, 2));
    }
} 