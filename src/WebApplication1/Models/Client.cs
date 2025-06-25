using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Others;

public abstract class Client
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [Phone]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;
}

public class IndividualClient : Client
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "PESEL must be exactly 11 digits")]
    public string Pesel { get; set; } = string.Empty;
    
    public bool IsDeleted { get; set; } = false;
}

public class CompanyClient : Client
{
    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;
    
    [Required]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "KRS number must be exactly 10 digits")]
    public string KrsNumber { get; set; } = string.Empty;
} 