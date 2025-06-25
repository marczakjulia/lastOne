using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Others;

public class CreateIndividualClientDto : CreateClientDto
{
    [Required]
    [MaxLength(50)]
    public required string FirstName { get; set; } 
    
    [Required]
    [MaxLength(50)]
    public required string LastName { get; set; }
    
    [Required]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "PESEL must be exactly 11 digits")]
    public required string Pesel { get; set; } 
}
