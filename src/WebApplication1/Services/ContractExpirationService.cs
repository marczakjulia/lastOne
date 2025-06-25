using Microsoft.EntityFrameworkCore;
using WebApplication1.Others;

namespace WebApplication1.Services;

public class ContractExpirationService : IContractExpirationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ContractExpirationService> _logger;

    public ContractExpirationService(ApplicationDbContext context, ILogger<ContractExpirationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> ProcessExpiredContractsAsync()
    {
        try
        {
            var potentiallyExpiredContracts = await _context.Contract
                .Include(c => c.Payments)
                .Where(c => c.Status == ContractStatus.Created && 
                           DateTime.UtcNow > c.EndDate)
                .ToListAsync();
            var expiredContracts = potentiallyExpiredContracts
                .Where(c => !c.IsFullyPaid)
                .ToList();

            var processedCount = 0;

            foreach (var contract in expiredContracts)
            {
                await ProcessSingleExpiredContract(contract);
                processedCount++;
            }

            if (processedCount > 0)
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Processed {processedCount} expired contracts with automatic refunds");
            }

            return processedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing expired contracts");
            throw;
        }
    }

    public async Task<bool> ProcessExpiredContractAsync(int contractId)
    {
        try
        {
            var contract = await _context.Contract
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c => c.Id == contractId);

            if (contract == null)
            {
                _logger.LogWarning($"Contract {contractId} not found");
                return false;
            }
            if (contract.Status != ContractStatus.Created || 
                DateTime.UtcNow <= contract.EndDate || 
                contract.IsFullyPaid)
            {
                return false; 
            }

            await ProcessSingleExpiredContract(contract);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Processed expired contract {contractId} with automatic refunds");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing expired contract {contractId}");
            throw;
        }
    }

    private async Task ProcessSingleExpiredContract(Contract contract)
    {
        contract.Status = ContractStatus.Expired;
 
        
        var paymentsToRefund = contract.Payments
            .Where(p => p.Status == PaymentStatus.Completed)
            .ToList();
            
        foreach (var payment in paymentsToRefund)
        {
            payment.Status = PaymentStatus.Refunded;
            
            _logger.LogInformation($"Auto-refunded payment {payment.Id} of {payment.Amount:C} for expired contract {contract.Id}");
        }
        var totalRefundAmount = paymentsToRefund.Sum(p => p.Amount);
        _logger.LogInformation($"Contract {contract.Id} expired - refunded {paymentsToRefund.Count} payments totaling {totalRefundAmount:C}");
    }
} 