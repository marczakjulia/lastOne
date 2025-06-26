using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using WebApplication1.Others;
using Xunit;

namespace TestProject1;

public class BusinessLogicTests 
{
    private readonly ApplicationDbContext _context;

    public BusinessLogicTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new ApplicationDbContext(options);
        AddData();
    }

    private void AddData()
    {
        _context.IndividualClient.AddRange(
            new IndividualClient { Id = 1, FirstName = "Jan", LastName = "Kowalski", Pesel = "80010112345", Address = "ul. Marszałkowska 123", Email = "jan@test.com", PhoneNumber = "+48123456789", IsDeleted = false },
            new IndividualClient { Id = 2, FirstName = "Anna", LastName = "Nowak", Pesel = "85051567890", Address = "ul. Krakowska 456", Email = "anna@test.com", PhoneNumber = "+48987654321", IsDeleted = false },
            new IndividualClient { Id = 3, FirstName = "Piotr", LastName = "Wiśniewski", Pesel = "90030398765", Address = "ul. Gdańska 789", Email = "piotr@test.com", PhoneNumber = "+48555666777", IsDeleted = true }
        );
        
        _context.CompanyClient.AddRange(
            new CompanyClient { Id = 1, CompanyName = "TechCorp Solutions", KrsNumber = "1234567890", Address = "ul. Długa 1", Email = "tech@corp.pl", PhoneNumber = "+48111222333" },
            new CompanyClient { Id = 2, CompanyName = "Innovation Systems", KrsNumber = "2345678901", Address = "ul. Krótka 5", Email = "info@innovation.pl", PhoneNumber = "+48222333444" }
        );
        
        _context.SoftwareSystem.AddRange(
            new SoftwareSystem { Id = 1, Name = "Financial Analytics Pro", Description = "gd",CurrentVersion = "2.1.0", Category = "Finance", PricingType = PricingType.Both, UpfrontPrice = 2999.99m, SubscriptionPrice = 299.99m },
            new SoftwareSystem { Id = 2, Name = "CRM Business Suite",Description = "GFd",CurrentVersion = "3.2.1", Category = "Business", PricingType = PricingType.Upfront, UpfrontPrice = 4999.99m }
        );
        
        _context.Discount.AddRange(
            new Discount { Id = 1, Name = "Spring Promotion", PercentageValue = 15.0m, Description = "fd", DiscountType = DiscountType.Upfront, StartDate = DateTime.Now.AddDays(-30), EndDate = DateTime.Now.AddDays(30), SoftwareSystemId = 1, IsActive = true },
            new Discount { Id = 2, Name = "Enterprise Deal", PercentageValue = 10.0m, Description = "fd", DiscountType = DiscountType.Upfront, StartDate = DateTime.Now.AddDays(-15), EndDate = DateTime.Now.AddDays(45), SoftwareSystemId = 2, IsActive = true }
        );
        
        _context.Contract.AddRange(
            new Contract { Id = 1, ClientId = 1, ClientType = "individual", SoftwareSystemId = 1, SoftwareVersion = "2.1.0", StartDate = DateTime.Today.AddDays(5), EndDate = DateTime.Today.AddDays(20), SupportYears = 2, BasePrice = 3999.99m, FinalPrice = 3059.99m, AppliedDiscountPercentage = 15.0m, AppliedDiscountAmount = 599.99m, IsReturningClientDiscountApplied = true, ReturningClientDiscountAmount = 170.0m, Status = ContractStatus.Signed },
            new Contract { Id = 2, ClientId = 1, ClientType = "company", SoftwareSystemId = 2, SoftwareVersion = "3.2.1", StartDate = DateTime.Today.AddDays(2), EndDate = DateTime.Today.AddDays(12), SupportYears = 1, BasePrice = 4999.99m, FinalPrice = 4499.99m, AppliedDiscountPercentage = 10.0m, AppliedDiscountAmount = 500.0m, Status = ContractStatus.Created },
            new Contract { Id = 3, ClientId = 2, ClientType = "individual", SoftwareSystemId = 1, SoftwareVersion = "2.1.0", StartDate = DateTime.Today.AddDays(-10), EndDate = DateTime.Today.AddDays(-2), SupportYears = 1, BasePrice = 2999.99m, FinalPrice = 2549.99m, Status = ContractStatus.Expired }
        );
        
        _context.Payment.AddRange(
            new Payment { Id = 1, ContractId = 1, Amount = 1500.00m, Status = PaymentStatus.Completed, PaymentMethod = "Credit Card" },
            new Payment { Id = 2, ContractId = 1, Amount = 1559.99m, Status = PaymentStatus.Completed, PaymentMethod = "Bank Transfer" },
            new Payment { Id = 3, ContractId = 2, Amount = 2000.00m, Status = PaymentStatus.Completed, PaymentMethod = "Credit Card" },
            new Payment { Id = 4, ContractId = 3, Amount = 1000.00m, Status = PaymentStatus.Refunded, PaymentMethod = "Credit Card" }
        );

        _context.SaveChanges();
    }
    
    [Fact]
    public void ShouldReturnActiveIndividualClientsCount()
    {
        var clients = _context.IndividualClient.ToList();
        var activeCount = clients.Count(c => !c.IsDeleted);
        activeCount.Should().Be(2);
    }
    
    [Fact]
    public void ShouldReturnCompanyClientsWithValidKrs()
    {
        var companies = _context.CompanyClient.ToList();
        var validKrs = companies.Where(c => c.KrsNumber.Length == 10).ToList();
        validKrs.Should().HaveCount(2);
        validKrs.Should().OnlyContain(c => c.KrsNumber.Length == 10);
    }
    
    [Fact]
    public void ShouldReturnMaxContractValue()
    {
        var contracts = _context.Contract.ToList();
        decimal maxValue = contracts.Max(c => c.FinalPrice);
        maxValue.Should().Be(4499.99m);
    }
    
    [Fact]
    public void ShouldReturnMinBasePriceForSignedContracts()
    {
        var contracts = _context.Contract.ToList();
        decimal minBasePrice = contracts.Where(c => c.Status == ContractStatus.Signed).Min(c => c.BasePrice);
        minBasePrice.Should().Be(3999.99m);
    }
    
    [Fact]
    public void ShouldReturnFirstTwoContractsByStartDate()
    {
        var contracts = _context.Contract.ToList();
        var firstTwo = contracts.OrderBy(c => c.StartDate).Take(2).ToList();
        firstTwo.Should().HaveCount(2);
        Assert.True(firstTwo[0].StartDate <= firstTwo[1].StartDate);
    }
    
    [Fact]
    public void ShouldReturnDistinctContractStatuses()
    {
        var contracts = _context.Contract.ToList();
        var statuses = contracts.Select(c => c.Status).Distinct().ToList();
        statuses.Should().Contain(ContractStatus.Signed);
        statuses.Should().Contain(ContractStatus.Created);
        statuses.Should().Contain(ContractStatus.Expired);
    }
    
    [Fact]
    public void ShouldReturnContractsWithAppliedDiscounts()
    {
        var contracts = _context.Contract.ToList();
        var withDiscounts = contracts.Where(c => c.AppliedDiscountPercentage.HasValue).ToList();
        withDiscounts.Should().OnlyContain(c => c.AppliedDiscountPercentage.HasValue);
        withDiscounts.Should().HaveCount(2);
    }
    
    [Fact]
    public void AllActiveContractsShouldHaveValidDuration()
    {
        var contracts = _context.Contract.ToList();
        var activeContracts = contracts.Where(c => c.Status != ContractStatus.Expired && c.Status != ContractStatus.Cancelled);
        var result = activeContracts.All(c => c.IsValidTimeRange());
        result.Should().BeTrue();
    }
    
    [Fact]
    public void ShouldFindAnyContractWithReturningClientDiscount()
    {
        var contracts = _context.Contract.ToList();
        var result = contracts.Any(c => c.IsReturningClientDiscountApplied);
        result.Should().BeTrue();
    }
    
    [Fact]
    public void ShouldJoinContractsWithPayments()
    {
        var contracts = _context.Contract.Include(c => c.Payments).ToList();
        var payments = _context.Payment.ToList();
        
        var result = from c in contracts
                     join p in payments on c.Id equals p.ContractId
                     select new { ContractId = c.Id, PaymentAmount = p.Amount, PaymentStatus = p.Status };
        
        result.Should().Contain(r => r.ContractId == 1 && r.PaymentAmount == 1500.00m && r.PaymentStatus == PaymentStatus.Completed);
    }
    
    [Fact]
    public void ShouldCalculateTotalRevenueFromCompletedPayments()
    {
        var payments = _context.Payment.ToList();
        decimal totalRevenue = payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);
        totalRevenue.Should().Be(5059.99m); // 1500 + 1559.99 + 2000
    }
    
    [Fact]
    public void ShouldCalculateAveragePaymentAmountPerContract()
    {
        var payments = _context.Payment.ToList();
        var contracts = _context.Contract.ToList();
        
        var result = from c in contracts
                     join p in payments on c.Id equals p.ContractId into contractPayments
                     where contractPayments.Any()
                     select new { 
                         ContractId = c.Id, 
                         AveragePayment = contractPayments.Where(cp => cp.Status == PaymentStatus.Completed).Average(cp => cp.Amount) 
                     };
        
        result.Should().Contain(r => r.ContractId == 1 && Math.Abs(r.AveragePayment - 1529.995m) < 0.01m);
    }
    
    [Fact]
    public void ShouldAnalyzeContractPaymentStatus()
    {
        var contracts = _context.Contract.Include(c => c.Payments).ToList();
        
        var result = from c in contracts
                     let TotalPaid = c.Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount)
                     let RemainingAmount = c.FinalPrice - TotalPaid
                     select new { 
                         ContractId = c.Id, 
                         IsFullyPaid = TotalPaid >= c.FinalPrice,
                         RemainingAmount = RemainingAmount,
                         Status = c.Status
                     };
        
        result.Should().Contain(r => r.ContractId == 1 && r.IsFullyPaid && r.Status == ContractStatus.Signed);
    }
    
    [Fact]
    public void ShouldJoinClientsContractsAndPayments()
    {
        var individualClients = _context.IndividualClient.ToList();
        var companyClients = _context.CompanyClient.ToList();
        var contracts = _context.Contract.Include(c => c.Payments).ToList();
        var individualResult = from ic in individualClients
                              join c in contracts on ic.Id equals c.ClientId
                              where c.ClientType == "individual"
                              let TotalPaid = c.Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount)
                              select new { 
                                  ClientName = $"{ic.FirstName} {ic.LastName}", 
                                  ClientType = "Individual",
                                  ContractValue = c.FinalPrice,
                                  TotalPaid = TotalPaid
                              };
        var companyResult = from cc in companyClients
                           join c in contracts on cc.Id equals c.ClientId
                           where c.ClientType == "company"
                           let TotalPaid = c.Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount)
                           select new { 
                               ClientName = cc.CompanyName, 
                               ClientType = "Company",
                               ContractValue = c.FinalPrice,
                               TotalPaid = TotalPaid
                           };
        
        var allResults = individualResult.Concat(companyResult).ToList();
        allResults.Should().Contain(r => r.ClientName == "Jan Kowalski" && r.ClientType == "Individual" && r.TotalPaid == 3059.99m);
    }
    
    [Fact]
    public void ShouldValidateSupportYearsPricingCalculation()
    {
        var contracts = _context.Contract.ToList();
        var softwareSystems = _context.SoftwareSystem.ToList();
        
        var result = from c in contracts
                     join s in softwareSystems on c.SoftwareSystemId equals s.Id
                     let ExpectedBasePrice = s.UpfrontPrice + ((c.SupportYears - 1) * 1000)
                     select new { 
                         ContractId = c.Id, 
                         SupportYears = c.SupportYears,
                         CalculatedBasePrice = ExpectedBasePrice,
                         ActualBasePrice = c.BasePrice
                     };
        
        result.Should().OnlyContain(r => Math.Abs(r.CalculatedBasePrice.Value - r.ActualBasePrice) < 0.01m);
    }
    
    [Fact]
    public void ShouldCalculateMonthlyRevenueProjection()
    {
        var contracts = _context.Contract.Include(c => c.Payments).ToList();
        
        var result = from c in contracts
                     where c.Status == ContractStatus.Signed
                     let CompletedRevenue = c.Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount)
                     group CompletedRevenue by c.StartDate.Month into monthlyGroup
                     select new { 
                         Month = monthlyGroup.Key,
                         TotalRevenue = monthlyGroup.Sum()
                     };
        
        result.Should().NotBeEmpty();
        result.Sum(r => r.TotalRevenue).Should().BeGreaterThan(0);
    }
    [Fact]
    public void ShouldAnalyzeExpiredContractsWithRefunds()
    {
        var contracts = _context.Contract.Include(c => c.Payments).ToList();
        
        var expiredWithRefunds = from c in contracts
                                where c.Status == ContractStatus.Expired
                                let RefundedAmount = c.Payments.Where(p => p.Status == PaymentStatus.Refunded).Sum(p => p.Amount)
                                let CompletedAmount = c.Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount)
                                select new { 
                                    ContractId = c.Id,
                                    RefundedAmount = RefundedAmount,
                                    CompletedAmount = CompletedAmount,
                                    HasRefunds = RefundedAmount > 0
                                };
        
        expiredWithRefunds.Should().Contain(r => r.ContractId == 3 && r.HasRefunds && r.RefundedAmount == 1000.00m);
    }
    
} 