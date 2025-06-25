using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Others;

public enum PricingType
{
    Subscription,
    Upfront,
    Both
}

public class SoftwareSystem
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public required string Name { get; set; } 
    
    [Required]
    [MaxLength(2000)]
    public  required string Description { get; set; } 
    
    [Required]
    [MaxLength(50)]
    public required string CurrentVersion { get; set; } 
    
    [Required]
    public string Category { get; set; }
    
    [Required]
    public PricingType PricingType { get; set; }
    
    public decimal? SubscriptionPrice { get; set; } 
    public decimal? UpfrontPrice { get; set; }
    
} 