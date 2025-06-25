using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Others;

public enum ContractStatus
{
    Created,     
    Signed,    
    Expired,     
    Cancelled    
}

public class Contract
{
    public int Id { get; set; }
    
    [Required]
    public required int ClientId { get; set; }
    
    [Required]
    public required string ClientType { get; set; }
    
    [Required]
    public int SoftwareSystemId { get; set; }
    public SoftwareSystem SoftwareSystem { get; set; } = null!;
    
    [Required]
    [MaxLength(50)]
    public string SoftwareVersion { get; set; } = string.Empty;
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [Required]
    [Range(1, 4)]
    public int SupportYears { get; set; } = 1; // Minimum 1 year, max 3 years (1 alrrady included)
    
    [Required]
    public decimal BasePrice { get; set; }
    
    [Required]
    public decimal FinalPrice { get; set; }
    
    public decimal? AppliedDiscountPercentage { get; set; }
    public decimal? AppliedDiscountAmount { get; set; }
    
    public int? AppliedDiscountId { get; set; }
    public Discount? AppliedDiscount { get; set; }
    
    public bool IsReturningClientDiscountApplied { get; set; } = false;
    public decimal? ReturningClientDiscountAmount { get; set; }
    
    [Required]
    public ContractStatus Status { get; set; } = ContractStatus.Created;
    
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    
    public decimal TotalPaidAmount => Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);
    public decimal RemainingAmount => FinalPrice - TotalPaidAmount;
    public bool IsFullyPaid => TotalPaidAmount >= FinalPrice;
    public bool IsExpired => DateTime.UtcNow > EndDate && Status == ContractStatus.Created;
    public bool CanAcceptPayment => Status == ContractStatus.Created && !IsExpired;
    
    public bool IsValidTimeRange()
    {
        var duration = EndDate - StartDate;
        return duration >= TimeSpan.FromDays(3) && duration <= TimeSpan.FromDays(30);
    }
} 