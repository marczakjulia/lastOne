using WebApplication1.Others;

namespace WebApplication1.DTOs.Employee;

public class EmployeeResponseDto
{
    public int Id { get; set; }
    public required string Login { get; set; }
    public EmployeeRole Role { get; set; }
}
