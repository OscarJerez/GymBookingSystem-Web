namespace GymBookingSystem.API.Domain;

public class RecurringClass
{
    public int Id { get; set; }
    public int OriginalClassId { get; set; }
    public DayOfWeek DayOfWeek { get; set; } // Monday, Tuesday, etc.
    public TimeSpan StartTime { get; set; } // Time component only
    public TimeSpan Duration { get; set; } // How long the class lasts
    public int Capacity { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } // When recurring starts
    public DateTime? EndDate { get; set; } // When recurring ends (null = indefinite)
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<GymClass> GeneratedClasses { get; set; } = new List<GymClass>();
}
