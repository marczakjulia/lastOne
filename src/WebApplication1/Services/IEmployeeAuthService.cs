using WebApplication1.DTOs.Employee;

namespace WebApplication1.Others;

public interface IEmployeeAuthService
{
    Task<EmployeeAuthResponseDto?> AuthenticateAsync(string login, string password);
    Task<Employee?> GetEmployeeByLoginAsync(string login);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
} 