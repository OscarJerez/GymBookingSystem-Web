using GymBookingSystem.API.Domain;

namespace GymBookingSystem.API.Services;

public interface IWaitlistService
{
    Task<ClassWaitlist> AddToWaitlistAsync(int userId, int classId);
    Task<bool> RemoveFromWaitlistAsync(int waitlistId);
    Task<List<ClassWaitlist>> GetUserWaitlistAsync(int userId);
    Task<List<ClassWaitlist>> GetClassWaitlistAsync(int classId);
    Task<bool> NotifyAndBookFromWaitlistAsync(int classId);
    Task<int> GetWaitlistPositionAsync(int userId, int classId);
}

public class WaitlistService : IWaitlistService
{
    private readonly GymBookingDbContext _context;
    private readonly ILogger<WaitlistService> _logger;

    public WaitlistService(GymBookingDbContext context, ILogger<WaitlistService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ClassWaitlist> AddToWaitlistAsync(int userId, int classId)
    {
        try
        {
            // Check if class exists
            var gymClass = await _context.GymClasses.FindAsync(classId);
            if (gymClass == null)
                throw new InvalidOperationException($"Class {classId} not found");

            // Check if already on waitlist
            var existing = await _context.ClassWaitlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ClassId == classId && w.Status == WaitlistStatus.Waiting);
            if (existing != null)
                throw new InvalidOperationException("Already on waitlist for this class");

            // Get next position
            var maxPosition = await _context.ClassWaitlists
                .Where(w => w.ClassId == classId && w.Status == WaitlistStatus.Waiting)
                .MaxAsync(w => (int?)w.Position) ?? 0;

            var waitlist = new ClassWaitlist
            {
                ClassId = classId,
                UserId = userId,
                Position = maxPosition + 1,
                AddedAt = DateTime.UtcNow
            };

            _context.ClassWaitlists.Add(waitlist);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User {userId} added to waitlist for class {classId} at position {waitlist.Position}");
            return waitlist;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding to waitlist: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> RemoveFromWaitlistAsync(int waitlistId)
    {
        try
        {
            var waitlist = await _context.ClassWaitlists.FindAsync(waitlistId);
            if (waitlist == null)
                throw new InvalidOperationException("Waitlist entry not found");

            waitlist.Status = WaitlistStatus.Cancelled;
            _context.ClassWaitlists.Update(waitlist);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Waitlist entry {waitlistId} removed");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error removing from waitlist: {ex.Message}");
            throw;
        }
    }

    public async Task<List<ClassWaitlist>> GetUserWaitlistAsync(int userId)
    {
        return await _context.ClassWaitlists
            .Where(w => w.UserId == userId && w.Status != WaitlistStatus.Cancelled)
            .OrderBy(w => w.Position)
            .ToListAsync();
    }

    public async Task<List<ClassWaitlist>> GetClassWaitlistAsync(int classId)
    {
        return await _context.ClassWaitlists
            .Where(w => w.ClassId == classId && w.Status == WaitlistStatus.Waiting)
            .OrderBy(w => w.Position)
            .ToListAsync();
    }

    public async Task<int> GetWaitlistPositionAsync(int userId, int classId)
    {
        var waitlist = await _context.ClassWaitlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ClassId == classId && w.Status == WaitlistStatus.Waiting);
        return waitlist?.Position ?? -1;
    }

    public async Task<bool> NotifyAndBookFromWaitlistAsync(int classId)
    {
        try
        {
            var gymClass = await _context.GymClasses.FindAsync(classId);
            if (gymClass == null)
                throw new InvalidOperationException("Class not found");

            var availableSpots = gymClass.AvailableSpots;
            if (availableSpots <= 0)
                return false;

            var waitlist = await _context.ClassWaitlists
                .Where(w => w.ClassId == classId && w.Status == WaitlistStatus.Waiting)
                .OrderBy(w => w.Position)
                .Take(availableSpots)
                .ToListAsync();

            foreach (var entry in waitlist)
            {
                entry.Status = WaitlistStatus.Notified;
                entry.NotifiedAt = DateTime.UtcNow;
            }

            _context.ClassWaitlists.UpdateRange(waitlist);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Notified {waitlist.Count} users from waitlist for class {classId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error notifying waitlist: {ex.Message}");
            throw;
        }
    }
}
