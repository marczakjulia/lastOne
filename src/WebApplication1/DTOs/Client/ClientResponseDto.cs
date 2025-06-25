namespace WebApplication1.Others;

public abstract class ClientResponseDto
{
    public int Id { get; set; }
    public required string Address { get; set; } 
    public required string Email { get; set; } 
    public required string PhoneNumber { get; set; } 
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
