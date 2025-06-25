using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Others;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/client")]
[Authorize] 
public class ClientsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ClientsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet("individual")]
    [Authorize(Roles = "Standard,Admin")] 
    public async Task<ActionResult<IEnumerable<IndividualClientResponseDto>>> GetIndividualClients()
    {
        var clients = await _context.IndividualClient
            .Where(c => !c.IsDeleted)
            .Select(c => new IndividualClientResponseDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Address = c.Address,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                Pesel = c.Pesel
            })
            .ToListAsync();

        return Ok(clients);
    }


    [HttpGet("company")]
    [Authorize(Roles = "Standard,Admin")]
    public async Task<ActionResult<IEnumerable<CompanyClientResponseDto>>> GetCompanyClients()
    {
        var clients = await _context.CompanyClient
            .Select(c => new CompanyClientResponseDto
            {
                Id = c.Id,
                CompanyName = c.CompanyName,
                Address = c.Address,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                KrsNumber = c.KrsNumber
            })
            .ToListAsync();

        return Ok(clients);
    }
    
    [HttpGet("individual/{id}")]
    [Authorize(Roles = "Standard,Admin")] 
    public async Task<ActionResult<IndividualClientResponseDto>> GetIndividualClient(int id)
    {
        var client = await _context.IndividualClient
            .Where(c => c.Id == id && !c.IsDeleted)
            .Select(c => new IndividualClientResponseDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Address = c.Address,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                Pesel = c.Pesel
            })
            .FirstOrDefaultAsync();

        if (client == null)
        {
            return NotFound();
        }

        return Ok(client);
    }
    
    [HttpGet("company/{id}")]
    [Authorize(Roles = "Standard,Admin")] 
    public async Task<ActionResult<CompanyClientResponseDto>> GetCompanyClient(int id)
    {
        var client = await _context.CompanyClient
            .Where(c => c.Id == id)
            .Select(c => new CompanyClientResponseDto
            {
                Id = c.Id,
                CompanyName = c.CompanyName,
                Address = c.Address,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                KrsNumber = c.KrsNumber
            })
            .FirstOrDefaultAsync();

        if (client == null)
        {
            return NotFound();
        }

        return Ok(client);
    }
    
    [HttpPost("individual")]
    [Authorize(Roles = "Standard,Admin")] 
    public async Task<ActionResult<IndividualClientResponseDto>> CreateIndividualClient(CreateIndividualClientDto createDto)
    {
        if (await _context.IndividualClient.AnyAsync(c => c.Pesel == createDto.Pesel))
        {
            return BadRequest("A client with this PESEL number already exists");
        }
        if (await _context.IndividualClient.AnyAsync(c => c.Email == createDto.Email) ||
            await _context.CompanyClient.AnyAsync(c => c.Email == createDto.Email))
        {
            return BadRequest("A client with this email already exists");
        }

        var client = new IndividualClient
        {
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Address = createDto.Address,
            Email = createDto.Email,
            PhoneNumber = createDto.PhoneNumber,
            Pesel = createDto.Pesel
        };

        _context.IndividualClient.Add(client);
        await _context.SaveChangesAsync();

        var response = new IndividualClientResponseDto
        {
            Id = client.Id,
            FirstName = client.FirstName,
            LastName = client.LastName,
            Address = client.Address,
            Email = client.Email,
            PhoneNumber = client.PhoneNumber,
            Pesel = client.Pesel
        };

        return CreatedAtAction(nameof(GetIndividualClient), new { id = client.Id }, response);
    }
    
    [HttpPost("company")]
    [Authorize(Roles = "Standard,Admin")] 
    public async Task<ActionResult<CompanyClientResponseDto>> CreateCompanyClient(CreateCompanyClientDto createDto)
    {
        if (await _context.CompanyClient.AnyAsync(c => c.KrsNumber == createDto.KrsNumber))
        {
            return BadRequest("A company with this KRS number already exists");
        }
        if (await _context.IndividualClient.AnyAsync(c => c.Email == createDto.Email) ||
            await _context.CompanyClient.AnyAsync(c => c.Email == createDto.Email))
        {
            return BadRequest("A client with this email already exists");
        }

        var client = new CompanyClient
        {
            CompanyName = createDto.CompanyName,
            Address = createDto.Address,
            Email = createDto.Email,
            PhoneNumber = createDto.PhoneNumber,
            KrsNumber = createDto.KrsNumber
        };

        _context.CompanyClient.Add(client);
        await _context.SaveChangesAsync();

        var response = new CompanyClientResponseDto
        {
            Id = client.Id,
            CompanyName = client.CompanyName,
            Address = client.Address,
            Email = client.Email,
            PhoneNumber = client.PhoneNumber,
            KrsNumber = client.KrsNumber
        };

        return CreatedAtAction(nameof(GetCompanyClient), new { id = client.Id }, response);
    }
    
    [HttpPut("individual/{id}")]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> UpdateIndividualClient(int id, UpdateIndividualClientDto updateDto)
    {
        var client = await _context.IndividualClient
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (client == null)
        {
            return NotFound();
        }
        
        if (await _context.IndividualClient.AnyAsync(c => c.Email == updateDto.Email && c.Id != id) ||
            await _context.CompanyClient.AnyAsync(c => c.Email == updateDto.Email))
        {
            return BadRequest("A client with this email already exists");
        }

        client.FirstName = updateDto.FirstName;
        client.LastName = updateDto.LastName;
        client.Address = updateDto.Address;
        client.Email = updateDto.Email;
        client.PhoneNumber = updateDto.PhoneNumber;

        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpPut("company/{id}")]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> UpdateCompanyClient(int id, UpdateCompanyClientDto updateDto)
    {
        var client = await _context.CompanyClient
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client == null)
        {
            return NotFound();
        }
        if (await _context.IndividualClient.AnyAsync(c => c.Email == updateDto.Email) ||
            await _context.CompanyClient.AnyAsync(c => c.Email == updateDto.Email && c.Id != id))
        {
            return BadRequest("A client with this email already exists");
        }

        client.CompanyName = updateDto.CompanyName;
        client.Address = updateDto.Address;
        client.Email = updateDto.Email;
        client.PhoneNumber = updateDto.PhoneNumber;

        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpDelete("individual/{id}")]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> DeleteIndividualClient(int id)
    {
        var client = await _context.IndividualClient
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (client == null)
        {
            return NotFound();
        }
        client.FirstName = "DELETED";
        client.LastName = "DELETED";
        client.Address = "DELETED";
        client.Email = $"deleted_{client.Id}@deleted.com";
        client.PhoneNumber = "000000000";
        client.IsDeleted = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
} 