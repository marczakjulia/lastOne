using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Others;

public enum PaymentStatus
{
    Pending,    
    Completed,  
    Failed,     
    Refunded    
}

public class Payment
{
    public int Id { get; set; }
    
    [Required]
    public int ContractId { get; set; }
    public Contract Contract { get; set; } = null!;
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
    
    [Required]
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    
    [Required]
    [MaxLength(200)]
    public required string PaymentMethod { get; set; } 
} 