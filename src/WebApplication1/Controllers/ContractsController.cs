using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTOs.Contract;
using WebApplication1.Others;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/contract")]
[Authorize] 
public class ContractsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IContractExpirationService _expirationService;

    public ContractsController(ApplicationDbContext context, IContractExpirationService expirationService)
    {
        _context = context;
        _expirationService = expirationService;
    }
    
    [HttpGet]
    [Authorize(Roles = "Standard,Admin")] 
    public async Task<ActionResult<IEnumerable<ContractResponseDto>>> GetAllContracts()
    {
        var contracts = await _context.Contract
            .Include(c => c.SoftwareSystem)
            .Select(c => new ContractResponseDto
            {
                Id = c.Id,
                ClientId = c.ClientId,
                ClientType = c.ClientType,
                SoftwareSystemName = c.SoftwareSystem.Name,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                FinalPrice = c.FinalPrice,
                Status = c.Status,
                IsFullyPaid = c.IsFullyPaid
            })
            .ToListAsync();

        return Ok(contracts);
    }
    
    [HttpGet("{id}")]
    [Authorize(Roles = "Standard,Admin")] 
    public async Task<ActionResult<ContractDetailResponseDto>> GetContract(int id)
    {
        var contract = await _context.Contract
            .Include(c => c.SoftwareSystem)
            .Include(c => c.Payments)
            .Where(c => c.Id == id)
            .Select(c => new ContractDetailResponseDto
            {
                Id = c.Id,
                ClientId = c.ClientId,
                ClientType = c.ClientType,
                SoftwareSystemName = c.SoftwareSystem.Name,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                FinalPrice = c.FinalPrice,
                Status = c.Status,
                IsFullyPaid = c.IsFullyPaid,
                Payments = c.Payments.Select(p => new PaymentResponseDto
                {
                    Id = p.Id,
                    ContractId = p.ContractId,
                    Amount = p.Amount,
                    Status = p.Status,
                    PaymentMethod = p.PaymentMethod
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (contract == null)
        {
            return NotFound();
        }

        return Ok(contract);
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ContractCreationResponseDto>> CreateContract(CreateContractDto createDto)
    {
       
        await _expirationService.ProcessExpiredContractsAsync();

        var duration = createDto.EndDate - createDto.StartDate;
        if (duration < TimeSpan.FromDays(3) || duration > TimeSpan.FromDays(30))
        {
            return BadRequest("Contract duration must be between 3 and 30 days.");
        }
        
        var clientExists = createDto.ClientType.ToLower() switch
        {
            "individual" => await _context.IndividualClient.AnyAsync(c => c.Id == createDto.ClientId && !c.IsDeleted),
            "company" => await _context.CompanyClient.AnyAsync(c => c.Id == createDto.ClientId),
            _ => false
        };

        if (!clientExists)
        {
            return BadRequest("Client does not exist");
        }
        
        var softwareSystem = await _context.SoftwareSystem
            .FirstOrDefaultAsync(s => s.Id == createDto.SoftwareSystemId);

        if (softwareSystem == null)
        {
            return BadRequest("Software system does not exist or is not active.");
        }

        if (softwareSystem.PricingType == PricingType.Subscription)
        {
            return BadRequest("This software system does not support upfront pricing.");
        }

        if (!softwareSystem.UpfrontPrice.HasValue)
        {
            return BadRequest("Software system does not have an upfront price configured.");
        }
        
        var hasActiveContract = await _context.Contract
            .AnyAsync(c => c.ClientId == createDto.ClientId && 
                          c.ClientType == createDto.ClientType &&
                          c.SoftwareSystemId == createDto.SoftwareSystemId &&
                          (c.Status == ContractStatus.Created || c.Status == ContractStatus.Signed));

        if (hasActiveContract)
        {
            return BadRequest("Client already has an active contract for this software system.");
        }
        
        var basePrice = softwareSystem.UpfrontPrice.Value + ((createDto.SupportYears - 1) * 1000); // Additional years cost 1000 PLN each
        
        var applicableDiscount = await _context.Discount
            .Where(d => d.SoftwareSystemId == createDto.SoftwareSystemId &&
                       d.IsActive &&
                       d.DiscountType == DiscountType.Upfront &&
                       d.StartDate <= DateTime.UtcNow &&
                       d.EndDate >= DateTime.UtcNow)
            .OrderByDescending(d => d.PercentageValue)
            .FirstOrDefaultAsync();

        var isReturningClient = await IsReturningClient(createDto.ClientId, createDto.ClientType);

        var finalPrice = basePrice;
        var appliedDiscountPercentage = 0m;
        var appliedDiscountAmount = 0m;
        var returningClientDiscountAmount = 0m;

        if (applicableDiscount != null)
        {
            appliedDiscountPercentage = applicableDiscount.PercentageValue;
            appliedDiscountAmount = basePrice * (applicableDiscount.PercentageValue / 100);
            finalPrice -= appliedDiscountAmount;
        }

        if (isReturningClient)
        {
            returningClientDiscountAmount = finalPrice * 0.05m; 
            finalPrice -= returningClientDiscountAmount;
        }

        var contract = new Contract
        {
            ClientId = createDto.ClientId,
            ClientType = createDto.ClientType,
            SoftwareSystemId = createDto.SoftwareSystemId,
            SoftwareVersion = createDto.SoftwareVersion,
            StartDate = createDto.StartDate,
            EndDate = createDto.EndDate,
            SupportYears = createDto.SupportYears,
            BasePrice = basePrice,
            FinalPrice = finalPrice,
            AppliedDiscountPercentage = appliedDiscountPercentage,
            AppliedDiscountAmount = appliedDiscountAmount,
            AppliedDiscountId = applicableDiscount?.Id,
            IsReturningClientDiscountApplied = isReturningClient,
            ReturningClientDiscountAmount = returningClientDiscountAmount,
            Status = ContractStatus.Created
        };

        _context.Contract.Add(contract);
        await _context.SaveChangesAsync();
        
        await _context.Entry(contract).Reference(c => c.SoftwareSystem).LoadAsync();
        await _context.Entry(contract).Reference(c => c.AppliedDiscount).LoadAsync();

        var response = new ContractCreationResponseDto
        {
            Id = contract.Id,
            ClientId = contract.ClientId,
            ClientType = contract.ClientType,
            SoftwareSystemId = contract.SoftwareSystemId,
            SoftwareSystemName = contract.SoftwareSystem.Name,
            SoftwareVersion = contract.SoftwareVersion,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            SupportYears = contract.SupportYears,
            BasePrice = contract.BasePrice,
            FinalPrice = contract.FinalPrice,
            AppliedDiscountPercentage = contract.AppliedDiscountPercentage,
            AppliedDiscountAmount = contract.AppliedDiscountAmount,
            AppliedDiscountName = contract.AppliedDiscount?.Name,
            IsReturningClientDiscountApplied = contract.IsReturningClientDiscountApplied,
            ReturningClientDiscountAmount = contract.ReturningClientDiscountAmount,
            Status = contract.Status.ToString(),
            TotalPaidAmount = contract.TotalPaidAmount,
            RemainingAmount = contract.RemainingAmount
        };

        return CreatedAtAction(nameof(GetContract), new { id = contract.Id }, response);
    }

    private async Task<bool> IsReturningClient(int clientId, string clientType)
    {
        var hasPreviousContracts = await _context.Contract
            .AnyAsync(c => c.ClientId == clientId && 
                          c.ClientType == clientType &&
                          c.Status == ContractStatus.Signed);

        return hasPreviousContracts; 
    }
} 