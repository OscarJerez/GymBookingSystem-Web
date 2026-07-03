namespace GymBookingSystem.API.DTOs;

public class WaitlistDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
    public int Position { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class JoinWaitlistRequest
{
    public int ClassId { get; set; }
}

public class RemoveWaitlistRequest
{
    public int WaitlistId { get; set; }
}
