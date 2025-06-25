namespace WebApplication1.Others;

public class ContractResponseDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public required string ClientType { get; set; } 
    public required string SoftwareSystemName { get; set; } 
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal FinalPrice { get; set; }
    public ContractStatus Status { get; set; }
    public bool IsFullyPaid { get; set; }
}
