using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Others;

public class CreateCompanyClientDto : CreateClientDto
{
    [Required]
    [MaxLength(200)]
    public required string CompanyName { get; set; }
    
    [Required]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "KRS number must be exactly 10 digits")]
    public required string KrsNumber { get; set; } 
}
