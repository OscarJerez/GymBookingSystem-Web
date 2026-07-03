namespace GymBookingSystem.API.DTOs;

public class BookingDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public DateTime BookedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CreateBookingRequest
{
    public int ClassId { get; set; }
}
