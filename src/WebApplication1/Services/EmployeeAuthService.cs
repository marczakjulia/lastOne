using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using WebApplication1.DTOs.Employee;
using WebApplication1.Tokens;

namespace WebApplication1.Others;

public class EmployeeAuthService : IEmployeeAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;

    public EmployeeAuthService(
        ApplicationDbContext context, 
        IConfiguration configuration,
        ITokenService tokenService)
    {
        _context = context;
        _configuration = configuration;
        _tokenService = tokenService;
    }

    public async Task<EmployeeAuthResponseDto?> AuthenticateAsync(string login, string password)
    {
        var employee = await GetEmployeeByLoginAsync(login);
        
        if (employee == null || !VerifyPassword(password, employee.Password))
        {
            return null;
        }

        var token = _tokenService.GenerateToken(employee.Login, employee.Role.ToString());

        return new EmployeeAuthResponseDto
        {
            Token = token,
            Employee = new EmployeeResponseDto
            {
                Id = employee.Id,
                Login = employee.Login,
                Role = employee.Role
            },
        };
    }

    public async Task<Employee?> GetEmployeeByLoginAsync(string login)
    {
        return await _context.Employee
            .FirstOrDefaultAsync(e => e.Login == login);
    }

    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var hashedInput = HashPassword(password);
        return hashedInput == hashedPassword;
    }
} 