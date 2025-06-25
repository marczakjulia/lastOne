using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Others;
using WebApplication1.Tokens;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);
var jwtConfigData = builder.Configuration.GetSection("Jwt");
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new Exception("No connection string");
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(connectionString));
builder.Services.Configure<JwtOptions>(jwtConfigData);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfigData["Issuer"],
        ValidAudience = jwtConfigData["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfigData["Key"])),
        ClockSkew = TimeSpan.FromMinutes(10)
    };
});

builder.Services.AddAuthorization(options =>
{ 
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("StandardUser", policy => policy.RequireRole("Standard", "Admin"));
});

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmployeeAuthService, EmployeeAuthService>();

builder.Services.AddScoped<IContractExpirationService, ContractExpirationService>();
builder.Services.AddHostedService<ContractExpirationBackgroundService>();

builder.Services.AddHttpClient<ICurrencyService, CurrencyService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();