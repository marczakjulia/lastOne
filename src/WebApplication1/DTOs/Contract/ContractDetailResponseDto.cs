namespace WebApplication1.Others;

public class ContractDetailResponseDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string ClientType { get; set; } = string.Empty;
    public string SoftwareSystemName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal FinalPrice { get; set; }
    public ContractStatus Status { get; set; }
    public bool IsFullyPaid { get; set; }
    public List<PaymentResponseDto> Payments { get; set; } = new();
}