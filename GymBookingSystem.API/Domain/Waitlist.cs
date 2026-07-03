namespace GymBookingSystem.API.Domain;

public class Waitlist
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ClassId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public int Position { get; set; }
    public WaitlistStatus Status { get; set; } = WaitlistStatus.Waiting;

    // Navigation
    public User? User { get; set; }
    public GymClass? GymClass { get; set; }
}

public enum WaitlistStatus
{
    Waiting,
    Notified,
    Expired,
    Cancelled
}
