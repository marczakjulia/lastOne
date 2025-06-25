using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Others;

public class UpdateIndividualClientDto : UpdateClientDto
{
    [Required]
    [MaxLength(50)]
    public required string FirstName { get; set; }
    
    [Required]
    [MaxLength(50)]
    public required string LastName { get; set; }
}