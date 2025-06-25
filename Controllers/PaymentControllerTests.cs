using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using WebApplication1.Controllers;
using WebApplication1.Others;

namespace TestProject1.Controllers;

public class PaymentsControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly PaymentsController _controller;

    public PaymentsControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _controller = new PaymentsController(_context);

        SeedTestData();
    }

    private void SeedTestData()
    {
        // Add Software System
        var softwareSystem = new SoftwareSystem
        {
            Id = 1,
            Name = "Test Software",
            Description = "Test Description",
            PricingType = PricingType.Upfront,
            UpfrontPrice = 3000m,
            IsActive = true
        };

        // Add Individual Client
        var individualClient = new IndividualClient
        {
            Id = 1,
            FirstName = "Jan",
            LastName = "Kowalski",
            PESEL = "85051512345",
            Address = "Test Address",
            Email = "jan@test.com",
            PhoneNumber = "+48123456789",
            IsDeleted = false
        };

        // Add Contract
        var contract = new Contract
        {
            Id = 1,
            ClientId = 1,
            ClientType = "individual",
            SoftwareSystemId = 1,
            SoftwareVersion = "1.0.0",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(15),
            SupportYears = 1,
            BasePrice = 3000m,
            FinalPrice = 3000m,
            Status = ContractStatus.Created
        };

        _context.SoftwareSystem.Add(softwareSystem);
        _context.IndividualClient.Add(individualClient);
        _context.Contract.Add(contract);
        _context.SaveChanges();
    }

    [Fact]
    public async Task CreatePayment_ShouldCreateValidPayment_WhenValidDataProvided()
    {
        // Arrange
        var createDto = new CreatePaymentDto
        {
            ContractId = 1,
            Amount = 1000m,
            PaymentMethod = "Credit Card"
        };

        // Act
        var result = await _controller.CreatePayment(createDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<PaymentResponseDto>().Subject;
        
        response.ContractId.Should().Be(1);
        response.Amount.Should().Be(1000m);
        response.PaymentMethod.Should().Be("Credit Card");
        response.Status.Should().Be(PaymentStatus.Pending);
        response.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task CreatePayment_ShouldReturnBadRequest_WhenContractDoesNotExist()
    {
        // Arrange
        var createDto = new CreatePaymentDto
        {
            ContractId = 999, // Non-existent contract
            Amount = 1000m,
            PaymentMethod = "Credit Card"
        };

        // Act
        var result = await _controller.CreatePayment(createDto);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Contract does not exist.");
    }

    [Fact]
    public async Task CreatePayment_ShouldReturnBadRequest_WhenContractIsExpired()
    {
        // Arrange
        var expiredContract = new Contract
        {
            Id = 2,
            ClientId = 1,
            ClientType = "individual",
            SoftwareSystemId = 1,
            SoftwareVersion = "1.0.0",
            StartDate = DateTime.Today.AddDays(-20),
            EndDate = DateTime.Today.AddDays(-1), // Expired
            SupportYears = 1,
            BasePrice = 3000m,
            FinalPrice = 3000m,
            Status = ContractStatus.Expired,
            ExpiredAt = DateTime.UtcNow.AddDays(-1)
        };

        _context.Contract.Add(expiredContract);
        await _context.SaveChangesAsync();

        var createDto = new CreatePaymentDto
        {
            ContractId = 2,
            Amount = 1000m,
            PaymentMethod = "Credit Card"
        };

        // Act
        var result = await _controller.CreatePayment(createDto);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Contract cannot accept payments. It may be expired, signed, or cancelled.");
    }

    [Fact]
    public async Task CreatePayment_ShouldReturnBadRequest_WhenAmountExceedsRemainingAmount()
    {
        // Arrange
        // Add existing payment to make remaining amount smaller
        var existingPayment = new Payment
        {
            Id = 1,
            ContractId = 1,
            Amount = 2500m,
            Status = PaymentStatus.Completed,
            PaymentMethod = "Bank Transfer",
            CreatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };

        _context.Payment.Add(existingPayment);
        await _context.SaveChangesAsync();

        var createDto = new CreatePaymentDto
        {
            ContractId = 1,
            Amount = 1000m, // Exceeds remaining amount (3000 - 2500 = 500)
            PaymentMethod = "Credit Card"
        };

        // Act
        var result = await _controller.CreatePayment(createDto);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Contain("Payment amount would exceed contract total");
    }

    [Fact]
    public async Task ProcessPayment_ShouldProcessValidPayment_WhenPaymentIsPending()
    {
        // Arrange
        var payment = new Payment
        {
            Id = 1,
            ContractId = 1,
            Amount = 1000m,
            Status = PaymentStatus.Pending,
            PaymentMethod = "Credit Card",
            CreatedAt = DateTime.UtcNow
        };

        _context.Payment.Add(payment);
        await _context.SaveChangesAsync();

        var processDto = new ProcessPaymentDto
        {
            PaymentId = 1,
            Status = PaymentStatus.Completed,
            Notes = "Payment processed successfully"
        };

        // Act
        var result = await _controller.ProcessPayment(1, processDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify payment was updated
        var updatedPayment = await _context.Payment.FindAsync(1);
        updatedPayment!.Status.Should().Be(PaymentStatus.Completed);
        updatedPayment.CompletedAt.Should().NotBeNull();
        updatedPayment.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task ProcessPayment_ShouldSignContract_WhenPaymentCompletesContract()
    {
        // Arrange
        var payment = new Payment
        {
            Id = 1,
            ContractId = 1,
            Amount = 3000m, // Full contract amount
            Status = PaymentStatus.Pending,
            PaymentMethod = "Credit Card",
            CreatedAt = DateTime.UtcNow
        };

        _context.Payment.Add(payment);
        await _context.SaveChangesAsync();

        var processDto = new ProcessPaymentDto
        {
            PaymentId = 1,
            Status = PaymentStatus.Completed,
            Notes = "Full payment processed"
        };

        // Act
        var result = await _controller.ProcessPayment(1, processDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify contract is signed
        var updatedContract = await _context.Contract.FindAsync(1);
        updatedContract!.Status.Should().Be(ContractStatus.Signed);
        updatedContract.SignedAt.Should().NotBeNull();
        updatedContract.SignedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task ProcessPayment_ShouldReturnNotFound_WhenPaymentDoesNotExist()
    {
        // Arrange
        var processDto = new ProcessPaymentDto
        {
            PaymentId = 999,
            Status = PaymentStatus.Completed
        };

        // Act
        var result = await _controller.ProcessPayment(999, processDto);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ProcessPayment_ShouldReturnBadRequest_WhenPaymentIsNotPending()
    {
        // Arrange
        var completedPayment = new Payment
        {
            Id = 1,
            ContractId = 1,
            Amount = 1000m,
            Status = PaymentStatus.Completed, // Already completed
            PaymentMethod = "Credit Card",
            CreatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };

        _context.Payment.Add(completedPayment);
        await _context.SaveChangesAsync();

        var processDto = new ProcessPaymentDto
        {
            PaymentId = 1,
            Status = PaymentStatus.Completed
        };

        // Act
        var result = await _controller.ProcessPayment(1, processDto);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Payment is not in pending status.");
    }

    [Fact]
    public async Task GetPaymentsByContract_ShouldReturnContractPayments_WhenContractExists()
    {
        // Arrange
        var payment1 = new Payment
        {
            Id = 1,
            ContractId = 1,
            Amount = 1000m,
            Status = PaymentStatus.Completed,
            PaymentMethod = "Credit Card",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            CompletedAt = DateTime.UtcNow.AddDays(-2)
        };

        var payment2 = new Payment
        {
            Id = 2,
            ContractId = 1,
            Amount = 500m,
            Status = PaymentStatus.Pending,
            PaymentMethod = "Bank Transfer",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        // Payment for different contract (should not be included)
        var otherContractPayment = new Payment
        {
            Id = 3,
            ContractId = 999,
            Amount = 200m,
            Status = PaymentStatus.Completed,
            PaymentMethod = "Cash",
            CreatedAt = DateTime.UtcNow
        };

        _context.Payment.AddRange(payment1, payment2, otherContractPayment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetPaymentsByContract(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var payments = okResult.Value.Should().BeAssignableTo<IEnumerable<PaymentResponseDto>>().Subject;
        
        payments.Should().HaveCount(2);
        payments.Should().OnlyContain(p => p.ContractId == 1);
        payments.Should().Contain(p => p.Amount == 1000m && p.Status == PaymentStatus.Completed);
        payments.Should().Contain(p => p.Amount == 500m && p.Status == PaymentStatus.Pending);
    }

    [Fact]
    public async Task GetPaymentsByContract_ShouldReturnEmptyList_WhenContractHasNoPayments()
    {
        // Act
        var result = await _controller.GetPaymentsByContract(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var payments = okResult.Value.Should().BeAssignableTo<IEnumerable<PaymentResponseDto>>().Subject;
        
        payments.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPaymentsByContract_ShouldReturnEmptyList_WhenContractDoesNotExist()
    {
        // Act
        var result = await _controller.GetPaymentsByContract(999);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var payments = okResult.Value.Should().BeAssignableTo<IEnumerable<PaymentResponseDto>>().Subject;
        
        payments.Should().BeEmpty();
    }

    [Fact]
    public async Task CreatePayment_ShouldAcceptPartialPayments_WhenContractIsNotFullyPaid()
    {
        // Arrange
        var existingPayment = new Payment
        {
            Id = 1,
            ContractId = 1,
            Amount = 1000m,
            Status = PaymentStatus.Completed,
            PaymentMethod = "Credit Card",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            CompletedAt = DateTime.UtcNow.AddDays(-1)
        };

        _context.Payment.Add(existingPayment);
        await _context.SaveChangesAsync();

        var createDto = new CreatePaymentDto
        {
            ContractId = 1,
            Amount = 500m, // Partial payment (remaining: 3000 - 1000 = 2000)
            PaymentMethod = "Bank Transfer"
        };

        // Act
        var result = await _controller.CreatePayment(createDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<PaymentResponseDto>().Subject;
        
        response.Amount.Should().Be(500m);
        response.Status.Should().Be(PaymentStatus.Pending);
    }

    [Fact]
    public async Task ProcessPayment_ShouldNotSignContract_WhenPaymentDoesNotCompleteContract()
    {
        // Arrange
        var payment = new Payment
        {
            Id = 1,
            ContractId = 1,
            Amount = 1000m, // Partial payment (contract total: 3000m)
            Status = PaymentStatus.Pending,
            PaymentMethod = "Credit Card",
            CreatedAt = DateTime.UtcNow
        };

        _context.Payment.Add(payment);
        await _context.SaveChangesAsync();

        var processDto = new ProcessPaymentDto
        {
            PaymentId = 1,
            Status = PaymentStatus.Completed
        };

        // Act
        var result = await _controller.ProcessPayment(1, processDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify contract is NOT signed
        var updatedContract = await _context.Contract.FindAsync(1);
        updatedContract!.Status.Should().Be(ContractStatus.Created); // Should remain created
        updatedContract.SignedAt.Should().BeNull();
    }

    [Fact]
    public async Task ProcessPayment_ShouldHandleFailedPayments_WhenStatusIsFailed()
    {
        // Arrange
        var payment = new Payment
        {
            Id = 1,
            ContractId = 1,
            Amount = 1000m,
            Status = PaymentStatus.Pending,
            PaymentMethod = "Credit Card",
            CreatedAt = DateTime.UtcNow
        };

        _context.Payment.Add(payment);
        await _context.SaveChangesAsync();

        var processDto = new ProcessPaymentDto
        {
            PaymentId = 1,
            Status = PaymentStatus.Failed,
            Notes = "Payment failed due to insufficient funds"
        };

        // Act
        var result = await _controller.ProcessPayment(1, processDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify payment was marked as failed
        var updatedPayment = await _context.Payment.FindAsync(1);
        updatedPayment!.Status.Should().Be(PaymentStatus.Failed);
        updatedPayment.FailedAt.Should().NotBeNull();
        updatedPayment.FailedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        updatedPayment.Notes.Should().Be("Payment failed due to insufficient funds");
    }

    [Fact]
    public async Task ProcessPayment_ShouldHandleRefundedPayments_WhenStatusIsRefunded()
    {
        // Arrange
        var payment = new Payment
        {
            Id = 1,
            ContractId = 1,
            Amount = 1000m,
            Status = PaymentStatus.Pending,
            PaymentMethod = "Credit Card",
            CreatedAt = DateTime.UtcNow
        };

        _context.Payment.Add(payment);
        await _context.SaveChangesAsync();

        var processDto = new ProcessPaymentDto
        {
            PaymentId = 1,
            Status = PaymentStatus.Refunded,
            Notes = "Customer requested refund"
        };

        // Act
        var result = await _controller.ProcessPayment(1, processDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify payment was marked as refunded
        var updatedPayment = await _context.Payment.FindAsync(1);
        updatedPayment!.Status.Should().Be(PaymentStatus.Refunded);
        updatedPayment.RefundedAt.Should().NotBeNull();
        updatedPayment.RefundedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        updatedPayment.Notes.Should().Be("Customer requested refund");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
} 