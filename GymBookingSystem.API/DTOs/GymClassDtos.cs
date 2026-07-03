namespace GymBookingSystem.API.DTOs;

public class GymClassDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Capacity { get; set; }
    public int BookedCount { get; set; }
    public int AvailableSpots { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string TimeRange { get; set; } = string.Empty;
}

public class CreateGymClassRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Capacity { get; set; }
    public string InstructorName { get; set; } = string.Empty;
}
