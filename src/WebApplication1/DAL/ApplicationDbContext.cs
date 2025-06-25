using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Others;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Employee> Employee { get; set; }
    public DbSet<IndividualClient> IndividualClient { get; set; }
    public DbSet<CompanyClient> CompanyClient { get; set; }
    public DbSet<SoftwareSystem> SoftwareSystem { get; set; }
    public DbSet<Discount> Discount { get; set; }
    public DbSet<Contract> Contract { get; set; }
    public DbSet<Payment> Payment { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Login).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Login).IsUnique();
            entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
        });
        
        modelBuilder.Entity<IndividualClient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Pesel).IsRequired().HasMaxLength(11);
            entity.HasIndex(e => e.Pesel).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
        });
        
        modelBuilder.Entity<CompanyClient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.KrsNumber).IsRequired().HasMaxLength(10);
            entity.HasIndex(e => e.KrsNumber).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
        });
        
        modelBuilder.Entity<SoftwareSystem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.SubscriptionPrice).HasPrecision(18, 2);
            entity.Property(e => e.UpfrontPrice).HasPrecision(18, 2);
        });
        
        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PercentageValue).HasPrecision(5, 2);
            
            entity.HasOne(d => d.SoftwareSystem)
                  .WithMany()
                  .HasForeignKey(d => d.SoftwareSystemId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BasePrice).HasPrecision(18, 2);
            entity.Property(e => e.FinalPrice).HasPrecision(18, 2);
            entity.Property(e => e.AppliedDiscountPercentage).HasPrecision(5, 2);
            entity.Property(e => e.AppliedDiscountAmount).HasPrecision(18, 2);
            entity.Property(e => e.ReturningClientDiscountAmount).HasPrecision(18, 2);
            
            entity.HasOne(c => c.SoftwareSystem)
                  .WithMany()
                  .HasForeignKey(c => c.SoftwareSystemId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(c => c.AppliedDiscount)
                  .WithMany()
                  .HasForeignKey(c => c.AppliedDiscountId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
        
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.HasOne(p => p.Contract)
                  .WithMany(c => c.Payments)
                  .HasForeignKey(p => p.ContractId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<Employee>().HasData(
            new Employee { Id = 1, Login = "admin", Password = "0192023a7bbd73250516f069df18b500", Role = EmployeeRole.Admin },
            new Employee { Id = 2, Login = "john.doe", Password = "482c811da5d5b4bc6d497ffa98491e38", Role = EmployeeRole.Standard },
            new Employee { Id = 3, Login = "jane.smith", Password = "482c811da5d5b4bc6d497ffa98491e38", Role = EmployeeRole.Standard }
        );
        
        modelBuilder.Entity<IndividualClient>().HasData(
            new IndividualClient { Id = 1, FirstName = "Jan", LastName = "Kowalski", Pesel = "80010112345", Address = "ul. Marszałkowska 123, 00-001 Warszawa", Email = "jan.kowalski@email.com", PhoneNumber = "+48123456789", IsDeleted = false },
            new IndividualClient { Id = 2, FirstName = "Anna", LastName = "Nowak", Pesel = "85051567890", Address = "ul. Krakowska 456, 31-001 Kraków", Email = "anna.nowak@email.com", PhoneNumber = "+48987654321", IsDeleted = false },
            new IndividualClient { Id = 3, FirstName = "Piotr", LastName = "Wiśniewski", Pesel = "90030398765", Address = "ul. Gdańska 789, 80-001 Gdańsk", Email = "piotr.wisniewski@email.com", PhoneNumber = "+48555666777", IsDeleted = false}
        );
        
        modelBuilder.Entity<CompanyClient>().HasData(
            new CompanyClient { Id = 1, CompanyName = "TechCorp Solutions Sp. z o.o.", KrsNumber = "1234567890", Address = "ul. Długa 1, 00-001 Warszawa", Email = "kontakt@techcorp.pl", PhoneNumber = "+48111222333" },
            new CompanyClient { Id = 2, CompanyName = "Innovation Systems S.A.", KrsNumber = "2345678901", Address = "ul. Krótka 5, 31-001 Kraków", Email = "info@innovationsystems.pl", PhoneNumber = "+48222333444"},
            new CompanyClient { Id = 3, CompanyName = "Digital Future Ltd.", KrsNumber = "3456789012", Address = "ul. Morska 15, 81-001 Gdynia", Email = "contact@digitalfuture.com", PhoneNumber = "+48333444555" }
        );
        
        modelBuilder.Entity<SoftwareSystem>().HasData(
            new SoftwareSystem { Id = 1, Name = "Financial Analytics Pro", Description = "Advanced financial reporting and analytics platform", CurrentVersion = "2.1.0", Category = "Finances", PricingType = PricingType.Both, SubscriptionPrice = 299.99m, UpfrontPrice = 2999.99m },
            new SoftwareSystem { Id = 2, Name = "EduTech Learning Platform", Description = "Comprehensive learning management system", CurrentVersion = "1.8.5", Category = "Education", PricingType = PricingType.Subscription, SubscriptionPrice = 199.99m, UpfrontPrice = null},
            new SoftwareSystem { Id = 3, Name = "CRM Business Suite", Description = "Customer relationship management system", CurrentVersion = "3.2.1", Category = "Business", PricingType = PricingType.Upfront, SubscriptionPrice = null, UpfrontPrice = 4999.99m }
        );
        
        var currentDate = DateTime.Now;
        var futureDate = currentDate.AddMonths(6);
        
        modelBuilder.Entity<Discount>().HasData(
            new Discount { Id = 1, Name = "Spring Promotion", Description = "Special discount for new customers", PercentageValue = 15.0m, DiscountType = DiscountType.Upfront, StartDate = currentDate.AddDays(-30), EndDate = futureDate, SoftwareSystemId = 1,  IsActive = true },
            new Discount { Id = 2, Name = "Education Discount", Description = "Special pricing for educational institutions", PercentageValue = 25.0m, DiscountType = DiscountType.Upfront, StartDate = currentDate.AddDays(-60), EndDate = futureDate.AddMonths(6), SoftwareSystemId = 2, IsActive = true },
            new Discount { Id = 3, Name = "Enterprise Deal", Description = "Volume discount for large enterprises", PercentageValue = 10.0m, DiscountType = DiscountType.Upfront, StartDate = currentDate.AddDays(-15), EndDate = futureDate.AddMonths(3), SoftwareSystemId = 3,  IsActive = true }
        );
        
        modelBuilder.Entity<Contract>().HasData(
            new Contract 
            { 
                Id = 1, 
                ClientId = 1, 
                ClientType = "individual", 
                SoftwareSystemId = 1, 
                SoftwareVersion = "2.1.0", 
                StartDate = currentDate.AddDays(5), 
                EndDate = currentDate.AddDays(20), 
                SupportYears = 2, 
                BasePrice = 3999.99m, 
                FinalPrice = 3059.99m, 
                AppliedDiscountPercentage = 15.0m,
                AppliedDiscountAmount = 599.99m,
                AppliedDiscountId = 1,
                IsReturningClientDiscountApplied = true,
                ReturningClientDiscountAmount = 170.0m,
                Status = ContractStatus.Signed, 
            },
            new Contract 
            { 
                Id = 2, 
                ClientId = 1, 
                ClientType = "company", 
                SoftwareSystemId = 2, 
                SoftwareVersion = "1.8.5", 
                StartDate = currentDate.AddDays(2), 
                EndDate = currentDate.AddDays(12), 
                SupportYears = 1, 
                BasePrice = 199.99m, 
                FinalPrice = 149.99m, 
                AppliedDiscountPercentage = 25.0m,
                AppliedDiscountAmount = 50.0m,
                AppliedDiscountId = 2,
                IsReturningClientDiscountApplied = false,
                ReturningClientDiscountAmount = null,
                Status = ContractStatus.Created, 
            },
            new Contract 
            { 
                Id = 3, 
                ClientId = 2, 
                ClientType = "individual", 
                SoftwareSystemId = 3, 
                SoftwareVersion = "3.2.1", 
                StartDate = currentDate.AddDays(7), 
                EndDate = currentDate.AddDays(30),
                SupportYears = 3, 
                BasePrice = 6999.99m,
                FinalPrice = 5994.99m,
                AppliedDiscountPercentage = 10.0m,
                AppliedDiscountAmount = 699.99m,
                AppliedDiscountId = 3,
                IsReturningClientDiscountApplied = true,
                ReturningClientDiscountAmount = 315.0m,
                Status = ContractStatus.Created, 
            },
            new Contract 
            { 
                Id = 4, 
                ClientId = 3, 
                ClientType = "individual", 
                SoftwareSystemId = 1, 
                SoftwareVersion = "2.1.0", 
                StartDate = currentDate.AddDays(-10), 
                EndDate = currentDate.AddDays(-2), 
                SupportYears = 1, 
                BasePrice = 2999.99m, 
                FinalPrice = 2549.99m, 
                AppliedDiscountPercentage = 15.0m,
                AppliedDiscountAmount = 449.99m,
                AppliedDiscountId = 1,
                IsReturningClientDiscountApplied = false,
                ReturningClientDiscountAmount = null,
                Status = ContractStatus.Created, 
            }
        );
        
        modelBuilder.Entity<Payment>().HasData(
            new Payment 
            { 
                Id = 1, 
                ContractId = 1, 
                Amount = 1500.00m, 
                Status = PaymentStatus.Completed, 
                PaymentMethod = "Credit Card"
            },
            new Payment 
            { 
                Id = 2, 
                ContractId = 1, 
                Amount = 1559.99m, 
                Status = PaymentStatus.Completed, 
                PaymentMethod = "Bank Transfer"
            },
            new Payment 
            { 
                Id = 3, 
                ContractId = 2, 
                Amount = 75.00m, 
                Status = PaymentStatus.Completed, 
                PaymentMethod = "Credit Card"
            },
            new Payment 
            { 
                Id = 4, 
                ContractId = 3, 
                Amount = 2000.00m, 
                Status = PaymentStatus.Completed, 
                PaymentMethod = "Bank Transfer"
            },
            new Payment 
            { 
                Id = 5, 
                ContractId = 4, 
                Amount = 1000.00m, 
                Status = PaymentStatus.Completed, 
                PaymentMethod = "Credit Card"
            }
        );
    }
}