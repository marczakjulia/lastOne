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
    public string Login { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty; // Will be hashed
    
    [Required]
    public EmployeeRole Role { get; set; } = EmployeeRole.Standard;
} 