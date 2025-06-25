using WebApplication1.Others;

public class SoftwareSystemResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CurrentVersion { get; set; }
    public string Category { get; set; }
    public PricingType PricingType { get; set; }
    public decimal? SubscriptionPrice { get; set; }
    public decimal? UpfrontPrice { get; set; }
} 