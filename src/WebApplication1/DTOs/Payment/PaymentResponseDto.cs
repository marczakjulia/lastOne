namespace WebApplication1.Others;

public class PaymentResponseDto
{
    public int Id { get; set; }
    public int ContractId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public required string PaymentMethod { get; set; }
} 