namespace GymBookingSystem.API.Domain;

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ClassId { get; set; }
    public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    public BookingStatus Status { get; set; } = BookingStatus.Active;

    // Navigation
    public User? User { get; set; }
    public GymClass? GymClass { get; set; }
}

public enum BookingStatus
{
    Active,
    Cancelled,
    Completed
}
