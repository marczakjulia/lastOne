namespace WebApplication1.DTOs.Employee;

public class EmployeeAuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public EmployeeResponseDto Employee { get; set; } 
    public DateTime ExpiresAt { get; set; }
} 