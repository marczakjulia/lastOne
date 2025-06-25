namespace WebApplication1.Services;

public interface IContractExpirationService
{
 
    Task<int> ProcessExpiredContractsAsync();
    
    Task<bool> ProcessExpiredContractAsync(int contractId);
} 