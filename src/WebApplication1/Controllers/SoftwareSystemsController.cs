using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Others;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/software")]
[Authorize] 
public class SoftwareSystemsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SoftwareSystemsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    [Authorize(Roles = "Standard,Admin")]
    public async Task<ActionResult<IEnumerable<SoftwareSystemResponseDto>>> GetAllSoftwareSystems()
    {
        var systems = await _context.SoftwareSystem
            .Select(s => new SoftwareSystemResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                CurrentVersion = s.CurrentVersion,
                Category = s.Category.ToString(),
                PricingType = s.PricingType,
                SubscriptionPrice = s.SubscriptionPrice,
                UpfrontPrice = s.UpfrontPrice
            })
            .ToListAsync();
        return Ok(systems);
    }
} 