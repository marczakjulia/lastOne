using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs.Employee;

public class EmployeeLoginDto
{
    [Required]
    [MaxLength(50)]
    public string Login { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;
}
