using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Others;

public abstract class CreateClientDto
{
    [Required]
    [MaxLength(500)]
    public required string Address { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public required string Email { get; set; }

    [Required]
    [Phone]
    [MaxLength(20)]
    public required string PhoneNumber { get; set; }
}