using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTOs.Employee;
using WebApplication1.Others;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/employee")]
public class EmployeeController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IEmployeeAuthService _authService;

    public EmployeeController(ApplicationDbContext context, IEmployeeAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<EmployeeAuthResponseDto>> Login(EmployeeLoginDto loginDto)
    {
        var authResult = await _authService.AuthenticateAsync(loginDto.Login, loginDto.Password);
        
        if (authResult == null)
        {
            return Unauthorized("Invalid login or password.");
        }

        return Ok(authResult);
    }

  
    [HttpPost("register")]
    public async Task<ActionResult<EmployeeResponseDto>> RegisterEmployee(EmployeeLoginDto registerDto)
    {
        
        var existingEmployee = await _authService.GetEmployeeByLoginAsync(registerDto.Login);
        if (existingEmployee != null)
        {
            return BadRequest("Employee with this login already exists.");
        }

        var hashedPassword = _authService.HashPassword(registerDto.Password);
        
        var employee = new Employee
        {
            Login = registerDto.Login,
            Password = hashedPassword,
            Role = EmployeeRole.Standard 
        };

        _context.Employee.Add(employee);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, new EmployeeResponseDto
        {
            Id = employee.Id,
            Login = employee.Login,
            Role = employee.Role
        });
    }


    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EmployeeResponseDto>> GetEmployeeById(int id)
    {
        var employee = await _context.Employee.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }
        return Ok(new EmployeeResponseDto
        {
            Id = employee.Id,
            Login = employee.Login,
            Role = employee.Role
        });
    }
} 