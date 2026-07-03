namespace GymBookingSystem.API.Domain;

public class GymClass
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Capacity { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public int BookedCount => Bookings.Count(b => b.Status == BookingStatus.Active);
    public bool IsFull => BookedCount >= Capacity;
    public int AvailableSpots => Capacity - BookedCount;
    
    public string GetStatus() => IsFull ? "FULL" : $"Spots left: {AvailableSpots}";
    public string GetTimeRange() => $"{StartTime:dddd HH:mm} - {EndTime:HH:mm}";
}
