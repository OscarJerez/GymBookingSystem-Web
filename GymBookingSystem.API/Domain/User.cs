namespace GymBookingSystem.API.Domain;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Member;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

public enum UserRole
{
    Member,
    Owner,
    Admin
}
