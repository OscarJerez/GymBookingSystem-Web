namespace GymBookingSystem.API.DTOs;

public class RecurringClassDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public string[] DaysOfWeek { get; set; } = Array.Empty<string>();
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public int GeneratedClassesCount { get; set; }
}

public class CreateRecurringClassRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty; // "06:00"
    public string EndTime { get; set; } = string.Empty;   // "07:00"
    public int Capacity { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public string[] DaysOfWeek { get; set; } = Array.Empty<string>(); // ["MON", "WED", "FRI"]
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class UpdateRecurringClassRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public int? Capacity { get; set; }
    public string? InstructorName { get; set; }
    public string[]? DaysOfWeek { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsActive { get; set; }
}
