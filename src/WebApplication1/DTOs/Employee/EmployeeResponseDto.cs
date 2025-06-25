using WebApplication1.Others;

namespace WebApplication1.DTOs.Employee;

public class EmployeeResponseDto
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public EmployeeRole Role { get; set; }
}
