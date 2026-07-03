namespace GymBookingSystem.API.Domain;

public class ClassWaitlist
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public int UserId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public WaitlistStatus Status { get; set; } = WaitlistStatus.Waiting;
    public DateTime? NotifiedAt { get; set; } // When user was notified of spot available
    public int Position { get; set; } // Position in queue

    // Navigation
    public GymClass? GymClass { get; set; }
    public User? User { get; set; }
}

public enum WaitlistStatus
{
    Waiting,
    Notified,
    Booked,
    Expired,
    Cancelled
}
