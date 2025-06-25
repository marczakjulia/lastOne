using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Others;

public class CreatePaymentDto
{
    [Required]
    public required int ContractId { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public required decimal Amount { get; set; }
    
    [Required]
    [MaxLength(200)]
    public required string PaymentMethod { get; set; } 
}
