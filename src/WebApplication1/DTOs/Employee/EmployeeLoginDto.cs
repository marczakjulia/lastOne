using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs.Employee;

public class EmployeeLoginDto
{
    [Required]
    [MaxLength(50)]
    public required string Login { get; set; }
    
    [Required]
    [MaxLength(100)]
    public required string Password { get; set; }
}
