using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Discount;
using WebApplication1.Others;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/discount")]
[Authorize] 
public class DiscountsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DiscountsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    [Authorize(Roles = "Standard,Admin")]
    public async Task<ActionResult<IEnumerable<DiscountResponseDto>>> GetDiscounts()
    {
        var discounts = await _context.Discount
            .Select(d => new DiscountResponseDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                PercentageValue = d.PercentageValue,
                DiscountType = d.DiscountType,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                SoftwareSystemId = d.SoftwareSystemId
            })
            .ToListAsync();
        return Ok(discounts);
    }
} 