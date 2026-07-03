using Microsoft.EntityFrameworkCore;
using GymBookingSystem.API.Domain;

namespace GymBookingSystem.API.Data;

public class GymBookingDbContext : DbContext
{
    public GymBookingDbContext(DbContextOptions<GymBookingDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<GymClass> GymClasses => Set<GymClass>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<User>()
            .HasMany(u => u.Bookings)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // GymClass
        modelBuilder.Entity<GymClass>()
            .HasMany(c => c.Bookings)
            .WithOne(b => b.GymClass)
            .HasForeignKey(b => b.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed data
        var adminHash = BCrypt.Net.BCrypt.HashPassword("admin123");
        var ownerHash = BCrypt.Net.BCrypt.HashPassword("owner123");

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@gymbooking.com",
                PasswordHash = adminHash,
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = 2,
                Username = "owner",
                Email = "owner@gymbooking.com",
                PasswordHash = ownerHash,
                Role = UserRole.Owner,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );

        var tomorrow = DateTime.UtcNow.AddDays(1).Date;
        modelBuilder.Entity<GymClass>().HasData(
            new GymClass
            {
                Id = 1,
                Name = "Morning Yoga",
                Description = "Relaxing yoga session to start your day",
                StartTime = tomorrow.AddHours(6),
                EndTime = tomorrow.AddHours(7),
                Capacity = 20,
                InstructorName = "Sarah",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new GymClass
            {
                Id = 2,
                Name = "HIIT Training",
                Description = "High-intensity interval training for max burn",
                StartTime = tomorrow.AddHours(17),
                EndTime = tomorrow.AddHours(18),
                Capacity = 15,
                InstructorName = "John",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new GymClass
            {
                Id = 3,
                Name = "Spinning Class",
                Description = "Indoor cycling workout",
                StartTime = tomorrow.AddHours(18),
                EndTime = tomorrow.AddHours(19),
                Capacity = 25,
                InstructorName = "Mike",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
