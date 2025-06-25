using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Others;

public class CreateContractDto
{
    [Required]
    public required int ClientId { get; set; }
    
    [Required]
    public required string ClientType { get; set; } 
    
    [Required]
    public required int SoftwareSystemId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public required string SoftwareVersion { get; set; }
    
    [Required]
    public required DateTime StartDate { get; set; }
    
    [Required]
    public required DateTime EndDate { get; set; }
    
    [Required]
    [Range(1, 3)]
    public required int SupportYears { get; set; } = 1;
}

