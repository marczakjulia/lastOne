using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Others;

public enum DiscountType
{
    Subscription,
    Upfront,
    Both
}

public class Discount
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Range(0, 100)]
    public decimal PercentageValue { get; set; }
    
    [Required]
    public DiscountType DiscountType { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public int? SoftwareSystemId { get; set; }
    public SoftwareSystem? SoftwareSystem { get; set; }
    
    public bool IsCurrentlyActive()
    {
        var now = DateTime.UtcNow;
        return IsActive && now >= StartDate && now <= EndDate;
    }
} 