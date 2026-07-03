using GymBookingSystem.API.Domain;

namespace GymBookingSystem.API.Services;

public interface IRecurringClassService
{
    Task<RecurringClass> CreateRecurringClassAsync(CreateRecurringClassRequest request);
    Task<bool> GenerateClassInstancesAsync(int recurringClassId);
    Task<List<RecurringClass>> GetActiveRecurringClassesAsync();
    Task<bool> UpdateRecurringClassAsync(int id, UpdateRecurringClassRequest request);
    Task<bool> StopRecurringClassAsync(int id);
}

public class RecurringClassService : IRecurringClassService
{
    private readonly GymBookingDbContext _context;
    private readonly ILogger<RecurringClassService> _logger;

    public RecurringClassService(GymBookingDbContext context, ILogger<RecurringClassService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<RecurringClass> CreateRecurringClassAsync(CreateRecurringClassRequest request)
    {
        try
        {
            if (request.DurationMinutes <= 0)
                throw new ArgumentException("Duration must be positive");

            var recurring = new RecurringClass
            {
                DayOfWeek = request.DayOfWeek,
                StartTime = request.StartTime,
                Duration = TimeSpan.FromMinutes(request.DurationMinutes),
                Capacity = request.Capacity,
                InstructorName = request.InstructorName,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.RecurringClasses.Add(recurring);
            await _context.SaveChangesAsync();

            await GenerateClassInstancesAsync(recurring.Id);

            _logger.LogInformation($"Created recurring class: {request.DayOfWeek} at {request.StartTime}");
            return recurring;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating recurring class: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> GenerateClassInstancesAsync(int recurringClassId)
    {
        try
        {
            var recurring = await _context.RecurringClasses.FindAsync(recurringClassId);
            if (recurring == null)
                throw new InvalidOperationException("Recurring class not found");

            var startDate = recurring.StartDate;
            var endDate = recurring.EndDate ?? DateTime.UtcNow.AddYears(1);

            var classes = new List<GymClass>();
            var currentDate = GetNextOccurrence(startDate, recurring.DayOfWeek);

            while (currentDate <= endDate)
            {
                var gymClass = new GymClass
                {
                    Name = $"{recurring.InstructorName}'s Class",
                    Description = "Auto-generated recurring class",
                    StartTime = currentDate.Date.Add(recurring.StartTime),
                    EndTime = currentDate.Date.Add(recurring.StartTime).Add(recurring.Duration),
                    Capacity = recurring.Capacity,
                    InstructorName = recurring.InstructorName,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                classes.Add(gymClass);
                currentDate = currentDate.AddDays(7);
            }

            _context.GymClasses.AddRange(classes);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Generated {classes.Count} class instances for recurring class {recurringClassId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating class instances: {ex.Message}");
            throw;
        }
    }

    public async Task<List<RecurringClass>> GetActiveRecurringClassesAsync()
    {
        return await _context.RecurringClasses
            .Where(r => r.IsActive && (r.EndDate == null || r.EndDate > DateTime.UtcNow))
            .ToListAsync();
    }

    public async Task<bool> UpdateRecurringClassAsync(int id, UpdateRecurringClassRequest request)
    {
        try
        {
            var recurring = await _context.RecurringClasses.FindAsync(id);
            if (recurring == null)
                throw new InvalidOperationException("Recurring class not found");

            recurring.Capacity = request.Capacity;
            recurring.InstructorName = request.InstructorName;
            recurring.EndDate = request.EndDate;

            _context.RecurringClasses.Update(recurring);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Updated recurring class {id}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating recurring class: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> StopRecurringClassAsync(int id)
    {
        try
        {
            var recurring = await _context.RecurringClasses.FindAsync(id);
            if (recurring == null)
                throw new InvalidOperationException("Recurring class not found");

            recurring.IsActive = false;
            recurring.EndDate = DateTime.UtcNow;

            _context.RecurringClasses.Update(recurring);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Stopped recurring class {id}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error stopping recurring class: {ex.Message}");
            throw;
        }
    }

    private DateTime GetNextOccurrence(DateTime from, DayOfWeek dayOfWeek)
    {
        var date = from.Date;
        while (date.DayOfWeek != dayOfWeek)
            date = date.AddDays(1);
        return date;
    }
}

public class CreateRecurringClassRequest
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public int DurationMinutes { get; set; }
    public int Capacity { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class UpdateRecurringClassRequest
{
    public int Capacity { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public DateTime? EndDate { get; set; }
}
