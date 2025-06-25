namespace WebApplication1.DTOs.Employee;

public class EmployeeAuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public EmployeeResponseDto Employee { get; set; } = new();
    public DateTime ExpiresAt { get; set; }
} 