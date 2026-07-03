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
    public int? ActiveMembershipId { get; set; }

    // Navigation
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Waitlist> Waitlists { get; set; } = new List<Waitlist>();
    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public Membership? ActiveMembership { get; set; }

    public bool HasActiveValidMembership => ActiveMembership != null && ActiveMembership.IsActive && ActiveMembership.EndDate > DateTime.UtcNow;
}

public enum UserRole
{
    Member,
    Owner,
    Admin
}
