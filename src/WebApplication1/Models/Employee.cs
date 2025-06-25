using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Others;

public enum EmployeeRole
{
    Standard,
    Admin
}

public class Employee
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public required string Login { get; set; }
    
    [Required]
    [MaxLength(100)]
    public required string Password { get; set; }
    
    [Required]
    public required EmployeeRole Role { get; set; }
} 
