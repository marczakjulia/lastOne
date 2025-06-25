namespace WebApplication1.DTOs.Contract;

public class ContractCreationResponseDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public required string ClientType { get; set; }
    public int SoftwareSystemId { get; set; }
    public required string SoftwareSystemName { get; set; } 
    public required string SoftwareVersion { get; set; } 
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int SupportYears { get; set; }
    public decimal BasePrice { get; set; }
    public decimal FinalPrice { get; set; }
    public decimal? AppliedDiscountPercentage { get; set; }
    public decimal? AppliedDiscountAmount { get; set; }
    public string? AppliedDiscountName { get; set; }
    public bool IsReturningClientDiscountApplied { get; set; }
    public decimal? ReturningClientDiscountAmount { get; set; }
    public required string Status { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
} 