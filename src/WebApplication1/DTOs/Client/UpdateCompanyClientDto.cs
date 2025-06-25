using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Others;

public class UpdateCompanyClientDto : UpdateClientDto
{
    [Required]
    [MaxLength(200)]
    public required string CompanyName { get; set; } 
}
