using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GymBookingSystem.API.Data;
using GymBookingSystem.API.Domain;
using GymBookingSystem.API.Services;
using GymBookingSystem.API.Exceptions;

namespace GymBookingSystem.Tests.Services;

public class PaymentServiceTests
{
    private GymBookingDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GymBookingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new GymBookingDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task GetMembershipPlans_ReturnsAllPlans()
    {
        // Arrange
        var context = GetInMemoryContext();
        var logger = new Mock<ILogger<PaymentService>>();
        var service = new PaymentService(context, logger.Object);

        // Act
        var plans = await service.GetMembershipPlansAsync();

        // Assert
        plans.Should().HaveCount(3);
        plans.Should().Contain(p => p.Type == "Basic");
        plans.Should().Contain(p => p.Type == "Premium");
        plans.Should().Contain(p => p.Type == "VIP");
    }

    [Fact]
    public async Task CreateMembership_InvalidType_ThrowsBadRequestException()
    {
        // Arrange
        var context = GetInMemoryContext();
        var user = new User { Id = 1, Username = "test" };
        context.Users.Add(user);
        context.SaveChanges();

        var logger = new Mock<ILogger<PaymentService>>();
        var service = new PaymentService(context, logger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => service.CreateMembershipAsync(1, "InvalidType"));
    }

    [Fact]
    public async Task CreateMembership_UserNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var context = GetInMemoryContext();
        var logger = new Mock<ILogger<PaymentService>>();
        var service = new PaymentService(context, logger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.CreateMembershipAsync(999, "Basic"));
    }

    [Fact]
    public async Task CreateMembership_Success_ReturnsValidMembership()
    {
        // Arrange
        var context = GetInMemoryContext();
        var user = new User { Id = 1, Username = "test", Email = "test@test.com", PasswordHash = "hash" };
        context.Users.Add(user);
        context.SaveChanges();

        var logger = new Mock<ILogger<PaymentService>>();
        var service = new PaymentService(context, logger.Object);

        // Act
        var result = await service.CreateMembershipAsync(1, "Premium");

        // Assert
        result.Should().NotBeNull();
        result.Type.Should().Be("Premium");
        result.Price.Should().Be(79m);
        result.IsActive.Should().BeTrue();
    }
}
