using Xunit;
using Moq;
using GymBookingSystem.API.Data;
using GymBookingSystem.API.Domain;
using GymBookingSystem.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymBookingSystem.API.Tests.Services;

public class WaitlistServiceTests
{
    private readonly GymBookingDbContext _context;
    private readonly IWaitlistService _waitlistService;
    private readonly Mock<ILogger<WaitlistService>> _loggerMock;

    public WaitlistServiceTests()
    {
        var options = new DbContextOptionsBuilder<GymBookingDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new GymBookingDbContext(options);
        _loggerMock = new Mock<ILogger<WaitlistService>>();
        _waitlistService = new WaitlistService(_context, _loggerMock.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = "hash",
            Role = UserRole.Member,
            IsActive = true
        };

        var gymClass = new GymClass
        {
            Id = 1,
            Name = "Test Class",
            Description = "Test",
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
            Capacity = 1,
            InstructorName = "John",
            IsActive = true
        };

        _context.Users.Add(user);
        _context.GymClasses.Add(gymClass);
        _context.SaveChanges();
    }

    [Fact]
    public async Task AddToWaitlist_Success()
    {
        // Act
        var result = await _waitlistService.AddToWaitlistAsync(1, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
        Assert.Equal(1, result.ClassId);
        Assert.Equal(WaitlistStatus.Waiting, result.Status);
        Assert.Equal(1, result.Position);
    }

    [Fact]
    public async Task AddToWaitlist_DuplicateEntry_Throws()
    {
        // Arrange
        await _waitlistService.AddToWaitlistAsync(1, 1);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _waitlistService.AddToWaitlistAsync(1, 1)
        );
    }

    [Fact]
    public async Task GetWaitlistPosition_Success()
    {
        // Arrange
        await _waitlistService.AddToWaitlistAsync(1, 1);

        // Act
        var position = await _waitlistService.GetWaitlistPositionAsync(1, 1);

        // Assert
        Assert.Equal(1, position);
    }

    [Fact]
    public async Task RemoveFromWaitlist_Success()
    {
        // Arrange
        var waitlist = await _waitlistService.AddToWaitlistAsync(1, 1);

        // Act
        var result = await _waitlistService.RemoveFromWaitlistAsync(waitlist.Id);

        // Assert
        Assert.True(result);
    }
}

public class RecurringClassServiceTests
{
    private readonly GymBookingDbContext _context;
    private readonly IRecurringClassService _recurringService;
    private readonly Mock<ILogger<RecurringClassService>> _loggerMock;

    public RecurringClassServiceTests()
    {
        var options = new DbContextOptionsBuilder<GymBookingDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new GymBookingDbContext(options);
        _loggerMock = new Mock<ILogger<RecurringClassService>>();
        _recurringService = new RecurringClassService(_context, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateRecurringClass_Success()
    {
        // Arrange
        var request = new CreateRecurringClassRequest
        {
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeSpan(10, 0, 0),
            DurationMinutes = 60,
            Capacity = 20,
            InstructorName = "John",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1)
        };

        // Act
        var result = await _recurringService.CreateRecurringClassAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(DayOfWeek.Monday, result.DayOfWeek);
        Assert.Equal(20, result.Capacity);
    }

    [Fact]
    public async Task CreateRecurringClass_InvalidDuration_Throws()
    {
        // Arrange
        var request = new CreateRecurringClassRequest
        {
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeSpan(10, 0, 0),
            DurationMinutes = -60,
            Capacity = 20,
            InstructorName = "John",
            StartDate = DateTime.UtcNow
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _recurringService.CreateRecurringClassAsync(request)
        );
    }

    [Fact]
    public async Task GetActiveRecurringClasses_Success()
    {
        // Arrange
        var request = new CreateRecurringClassRequest
        {
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeSpan(10, 0, 0),
            DurationMinutes = 60,
            Capacity = 20,
            InstructorName = "John",
            StartDate = DateTime.UtcNow
        };

        await _recurringService.CreateRecurringClassAsync(request);

        // Act
        var result = await _recurringService.GetActiveRecurringClassesAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.Single(result);
    }
}

public class PaymentServiceTests
{
    private readonly GymBookingDbContext _context;
    private readonly IPaymentService _paymentService;
    private readonly Mock<ILogger<PaymentService>> _loggerMock;
    private readonly Mock<IConfiguration> _configMock;

    public PaymentServiceTests()
    {
        var options = new DbContextOptionsBuilder<GymBookingDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new GymBookingDbContext(options);
        _loggerMock = new Mock<ILogger<PaymentService>>();
        _configMock = new Mock<IConfiguration>();
        _paymentService = new PaymentService(_context, _loggerMock.Object, _configMock.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = "hash",
            Role = UserRole.Member,
            IsActive = true
        };

        var plan = new MembershipPlan
        {
            Id = 1,
            Name = "Monthly",
            Description = "1 month",
            Price = 49.99m,
            DurationDays = 30,
            ClassesPerMonth = 0,
            IsActive = true
        };

        _context.Users.Add(user);
        _context.MembershipPlans.Add(plan);
        _context.SaveChanges();
    }

    [Fact]
    public async Task CreateMembershipSubscription_Success()
    {
        // Act
        var result = await _paymentService.CreateMembershipSubscriptionAsync(1, 1, "card");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
        Assert.Equal(1, result.PlanId);
        Assert.Equal(SubscriptionStatus.Active, result.Status);
    }

    [Fact]
    public async Task CreateMembershipSubscription_InvalidUser_Throws()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _paymentService.CreateMembershipSubscriptionAsync(999, 1, "card")
        );
    }

    [Fact]
    public async Task GetActiveSubscription_Success()
    {
        // Arrange
        await _paymentService.CreateMembershipSubscriptionAsync(1, 1, "card");

        // Act
        var result = await _paymentService.GetActiveSubscriptionAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
    }

    [Fact]
    public async Task CancelSubscription_Success()
    {
        // Arrange
        var subscription = await _paymentService.CreateMembershipSubscriptionAsync(1, 1, "card");

        // Act
        var result = await _paymentService.CancelSubscriptionAsync(subscription.Id);

        // Assert
        Assert.True(result);
    }
}
