using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Others;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/payment")]
[Authorize] 
public class PaymentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PaymentsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet("contract/{contractId}")]
    [Authorize(Roles = "Standard,Admin")] 
    public async Task<ActionResult<IEnumerable<PaymentResponseDto>>> GetPaymentsByContract(int contractId)
    {
        var payments = await _context.Payment
            .Include(p => p.Contract)
            .Where(p => p.ContractId == contractId)
            .Select(p => new PaymentResponseDto
            {
                Id = p.Id,
                ContractId = p.ContractId,
                Amount = p.Amount,
                Status = p.Status,
                PaymentMethod = p.PaymentMethod
            })
            .ToListAsync();

        return Ok(payments);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] 
    public async Task<ActionResult<object>> CreatePayment(CreatePaymentDto createDto)
    {
        var contract = await _context.Contract
            .FirstOrDefaultAsync(c => c.Id == createDto.ContractId);

        if (contract == null)
        {
            return BadRequest("Contract does not exist.");
        }
        
        if (contract.Status == ContractStatus.Expired)
        {
            return BadRequest("Contract is expired and cannot accept payments.");
        }

        if (contract.Status == ContractStatus.Cancelled)
        {
            return BadRequest("Contract is cancelled and cannot accept payments.");
        }
        
        var currentTotalPaid = await _context.Payment
            .Where(p => p.ContractId == createDto.ContractId && p.Status == PaymentStatus.Completed)
            .SumAsync(p => p.Amount);
            
        var remainingAmount = contract.FinalPrice - currentTotalPaid;
        
        if (remainingAmount <= 0)
        {
            return BadRequest($"Contract is already fully paid. Total paid: {currentTotalPaid:C}, Contract value: {contract.FinalPrice:C}");
        }
        
        if (DateTime.UtcNow > contract.EndDate && contract.Status == ContractStatus.Created)
        {
            return BadRequest("Contract has expired and cannot accept payments.");
        }
        
        if (createDto.Amount > remainingAmount)
        {
            return BadRequest($"Payment amount ({createDto.Amount:C}) exceeds remaining contract amount ({remainingAmount:C}). Maximum allowed payment: {remainingAmount:C}");
        }
        
        if (createDto.Amount <= 0)
        {
            return BadRequest("Payment amount must be greater than zero.");
        }
        
        var payment = new Payment
        {
            ContractId = createDto.ContractId,
            Amount = createDto.Amount,
            PaymentMethod = createDto.PaymentMethod,
            Status = PaymentStatus.Completed
        };

        _context.Payment.Add(payment);
        
        var newTotalPaid = currentTotalPaid + createDto.Amount;
        var newRemainingAmount = contract.FinalPrice - newTotalPaid;
        var isNowFullyPaid = newTotalPaid >= contract.FinalPrice;
        
        if (isNowFullyPaid && contract.Status == ContractStatus.Created)
        {
            contract.Status = ContractStatus.Signed;
        }

        await _context.SaveChangesAsync();
        
        var response = new
        {
            payment = new PaymentResponseDto
            {
                Id = payment.Id,
                ContractId = payment.ContractId,
                Amount = payment.Amount,
                Status = payment.Status,
                PaymentMethod = payment.PaymentMethod
            },
            contractInfo = new
            {
                contractId = contract.Id,
                totalContractValue = contract.FinalPrice,
                previouslyPaid = currentTotalPaid,
                thisPayment = createDto.Amount,
                newTotalPaid = newTotalPaid,
                remainingAmount = newRemainingAmount,
                isFullyPaid = isNowFullyPaid,
                contractStatus = contract.Status.ToString()
            }
        };

        return Ok(response);
    }

    [HttpGet]
    [Authorize(Roles = "Standard,Admin")] 
    public async Task<ActionResult<IEnumerable<PaymentResponseDto>>> GetAllPayments()
    {
        var payments = await _context.Payment
            .Include(p => p.Contract)
            .Select(p => new PaymentResponseDto
            {
                Id = p.Id,
                ContractId = p.ContractId,
                Amount = p.Amount,
                Status = p.Status,
                PaymentMethod = p.PaymentMethod
            })
            .ToListAsync();

        return Ok(payments);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Standard,Admin")] 
    public async Task<ActionResult<PaymentResponseDto>> GetPayment(int id)
    {
        var payment = await _context.Payment
            .Include(p => p.Contract)
            .Where(p => p.Id == id)
            .Select(p => new PaymentResponseDto
            {
                Id = p.Id,
                ContractId = p.ContractId,
                Amount = p.Amount,
                Status = p.Status,
                PaymentMethod = p.PaymentMethod
            })
            .FirstOrDefaultAsync();

        if (payment == null)
        {
            return NotFound();
        }

        return Ok(payment);
    }
} 