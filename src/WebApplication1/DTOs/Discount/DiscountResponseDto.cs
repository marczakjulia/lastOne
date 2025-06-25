using WebApplication1.Others;

namespace WebApplication1.Discount;

public class DiscountResponseDto
{
    public int Id { get; set; }
    public required string Name { get; set; } 
    public required string Description { get; set; } 
    public decimal PercentageValue { get; set; }
    public DiscountType DiscountType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? SoftwareSystemId { get; set; }
} 