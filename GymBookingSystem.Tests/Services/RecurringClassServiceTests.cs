using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GymBookingSystem.API.Data;
using GymBookingSystem.API.Domain;
using GymBookingSystem.API.Services;
using GymBookingSystem.API.Exceptions;
using GymBookingSystem.API.DTOs;

namespace GymBookingSystem.Tests.Services;

public class RecurringClassServiceTests
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
    public async Task CreateRecurringClass_InvalidTime_ThrowsBadRequestException()
    {
        // Arrange
        var context = GetInMemoryContext();
        var logger = new Mock<ILogger<RecurringClassService>>();
        var service = new RecurringClassService(context, logger.Object);

        var request = new CreateRecurringClassRequest
        {
            Name = "Test",
            StartTime = "invalid",
            EndTime = "07:00",
            DaysOfWeek = new[] { "MON" }
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => service.CreateRecurringClassAsync(request));
    }

    [Fact]
    public async Task CreateRecurringClass_StartTimeAfterEndTime_ThrowsBadRequestException()
    {
        // Arrange
        var context = GetInMemoryContext();
        var logger = new Mock<ILogger<RecurringClassService>>();
        var service = new RecurringClassService(context, logger.Object);

        var request = new CreateRecurringClassRequest
        {
            Name = "Test",
            StartTime = "08:00",
            EndTime = "07:00",
            DaysOfWeek = new[] { "MON" }
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => service.CreateRecurringClassAsync(request));
    }

    [Fact]
    public async Task CreateRecurringClass_NoDaysSelected_ThrowsBadRequestException()
    {
        // Arrange
        var context = GetInMemoryContext();
        var logger = new Mock<ILogger<RecurringClassService>>();
        var service = new RecurringClassService(context, logger.Object);

        var request = new CreateRecurringClassRequest
        {
            Name = "Test",
            StartTime = "06:00",
            EndTime = "07:00",
            DaysOfWeek = Array.Empty<string>()
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => service.CreateRecurringClassAsync(request));
    }

    [Fact]
    public async Task CreateRecurringClass_Success_ReturnsValidRecurringClass()
    {
        // Arrange
        var context = GetInMemoryContext();
        var logger = new Mock<ILogger<RecurringClassService>>();
        var service = new RecurringClassService(context, logger.Object);

        var request = new CreateRecurringClassRequest
        {
            Name = "Yoga",
            Description = "Morning Yoga",
            StartTime = "06:00",
            EndTime = "07:00",
            Capacity = 20,
            InstructorName = "Sarah",
            DaysOfWeek = new[] { "MON", "WED", "FRI" },
            StartDate = DateTime.UtcNow
        };

        // Act
        var result = await service.CreateRecurringClassAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Yoga");
        result.DaysOfWeek.Should().Contain("MON");
    }
}
