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

public class WaitlistServiceTests
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
    public async Task JoinWaitlist_ClassNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var context = GetInMemoryContext();
        var logger = new Mock<ILogger<WaitlistService>>();
        var service = new WaitlistService(context, logger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.JoinWaitlistAsync(1, 999));
    }

    [Fact]
    public async Task JoinWaitlist_UserAlreadyBooked_ThrowsBadRequestException()
    {
        // Arrange
        var context = GetInMemoryContext();
        var user = new User { Id = 1, Username = "test" };
        var tomorrow = DateTime.UtcNow.AddDays(1);
        var gymClass = new GymClass
        {
            Id = 1,
            Name = "Test Class",
            StartTime = tomorrow,
            EndTime = tomorrow.AddHours(1),
            Capacity = 1
        };
        var booking = new Booking
        {
            UserId = 1,
            ClassId = 1,
            Status = BookingStatus.Active,
            BookedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        context.GymClasses.Add(gymClass);
        context.Bookings.Add(booking);
        context.SaveChanges();

        var logger = new Mock<ILogger<WaitlistService>>();
        var service = new WaitlistService(context, logger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => service.JoinWaitlistAsync(1, 1));
    }

    [Fact]
    public async Task JoinWaitlist_ClassNotFull_ThrowsBadRequestException()
    {
        // Arrange
        var context = GetInMemoryContext();
        var tomorrow = DateTime.UtcNow.AddDays(1);
        var gymClass = new GymClass
        {
            Id = 1,
            Name = "Test Class",
            StartTime = tomorrow,
            EndTime = tomorrow.AddHours(1),
            Capacity = 10
        };

        context.GymClasses.Add(gymClass);
        context.SaveChanges();

        var logger = new Mock<ILogger<WaitlistService>>();
        var service = new WaitlistService(context, logger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => service.JoinWaitlistAsync(1, 1));
    }

    [Fact]
    public async Task JoinWaitlist_FullClass_SuccessfullyJoins()
    {
        // Arrange
        var context = GetInMemoryContext();
        var tomorrow = DateTime.UtcNow.AddDays(1);
        var gymClass = new GymClass
        {
            Id = 1,
            Name = "Test Class",
            StartTime = tomorrow,
            EndTime = tomorrow.AddHours(1),
            Capacity = 2
        };

        // Add 2 bookings to make class full
        context.GymClasses.Add(gymClass);
        context.Bookings.AddRange(
            new Booking { UserId = 2, ClassId = 1, Status = BookingStatus.Active, BookedAt = DateTime.UtcNow },
            new Booking { UserId = 3, ClassId = 1, Status = BookingStatus.Active, BookedAt = DateTime.UtcNow }
        );
        context.SaveChanges();

        var logger = new Mock<ILogger<WaitlistService>>();
        var service = new WaitlistService(context, logger.Object);

        // Act
        var result = await service.JoinWaitlistAsync(1, 1);

        // Assert
        result.Should().NotBeNull();
        result.ClassId.Should().Be(1);
        result.Position.Should().Be(1);
        result.Status.Should().Be("Waiting");
    }
}
