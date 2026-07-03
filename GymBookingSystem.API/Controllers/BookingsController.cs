using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using GymBookingSystem.API.Data;
using GymBookingSystem.API.Domain;
using GymBookingSystem.API.DTOs;

namespace GymBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly GymBookingDbContext _context;

    public BookingsController(GymBookingDbContext context)
    {
        _context = context;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetMyBookings()
    {
        var userId = GetUserId();
        var bookings = await _context.Bookings
            .Where(b => b.UserId == userId && b.Status == BookingStatus.Active)
            .Include(b => b.GymClass)
            .ToListAsync();

        return Ok(bookings.Select(b => ToDto(b)));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllBookings()
    {
        var bookings = await _context.Bookings
            .Include(b => b.GymClass)
            .ToListAsync();

        return Ok(bookings.Select(b => ToDto(b)));
    }

    [HttpPost]
    public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingRequest request)
    {
        var userId = GetUserId();
        var gymClass = await _context.GymClasses.FindAsync(request.ClassId);

        if (gymClass == null)
            return NotFound("Class not found");

        if (gymClass.IsFull)
            return BadRequest("Class is full");

        // Check if already booked
        var existingBooking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.UserId == userId && b.ClassId == request.ClassId && b.Status == BookingStatus.Active);

        if (existingBooking != null)
            return BadRequest("You are already booked for this class");

        var booking = new Booking
        {
            UserId = userId,
            ClassId = request.ClassId,
            BookedAt = DateTime.UtcNow,
            Status = BookingStatus.Active
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        booking.GymClass = gymClass;
        return CreatedAtAction(nameof(GetMyBookings), ToDto(booking));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var userId = GetUserId();
        var booking = await _context.Bookings.FindAsync(id);

        if (booking == null)
            return NotFound();

        if (booking.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();

        booking.Status = BookingStatus.Cancelled;
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private BookingDto ToDto(Booking b) => new()
    {
        Id = b.Id,
        UserId = b.UserId,
        ClassId = b.ClassId,
        ClassName = b.GymClass?.Name ?? "Unknown",
        BookedAt = b.BookedAt,
        Status = b.Status.ToString()
    };
}
