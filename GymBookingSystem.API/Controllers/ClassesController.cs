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
public class ClassesController : ControllerBase
{
    private readonly GymBookingDbContext _context;

    public ClassesController(GymBookingDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GymClassDto>>> GetAllClasses()
    {
        var classes = await _context.GymClasses
            .Where(c => c.IsActive)
            .OrderBy(c => c.StartTime)
            .ToListAsync();

        return Ok(classes.Select(c => ToDto(c)));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GymClassDto>> GetClass(int id)
    {
        var gymClass = await _context.GymClasses.FindAsync(id);
        if (gymClass == null)
            return NotFound();

        return Ok(ToDto(gymClass));
    }

    [Authorize(Roles = "Owner,Admin")]
    [HttpPost]
    public async Task<ActionResult<GymClassDto>> CreateClass(CreateGymClassRequest request)
    {
        if (request.StartTime >= request.EndTime)
            return BadRequest("Start time must be before end time");

        var gymClass = new GymClass
        {
            Name = request.Name,
            Description = request.Description,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Capacity = request.Capacity,
            InstructorName = request.InstructorName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.GymClasses.Add(gymClass);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetClass), new { id = gymClass.Id }, ToDto(gymClass));
    }

    [Authorize(Roles = "Owner,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClass(int id, CreateGymClassRequest request)
    {
        var gymClass = await _context.GymClasses.FindAsync(id);
        if (gymClass == null)
            return NotFound();

        if (request.StartTime >= request.EndTime)
            return BadRequest("Start time must be before end time");

        gymClass.Name = request.Name;
        gymClass.Description = request.Description;
        gymClass.StartTime = request.StartTime;
        gymClass.EndTime = request.EndTime;
        gymClass.Capacity = request.Capacity;
        gymClass.InstructorName = request.InstructorName;

        _context.GymClasses.Update(gymClass);
        await _context.SaveChangesAsync();

        return Ok(ToDto(gymClass));
    }

    [Authorize(Roles = "Owner,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClass(int id)
    {
        var gymClass = await _context.GymClasses.FindAsync(id);
        if (gymClass == null)
            return NotFound();

        gymClass.IsActive = false;
        _context.GymClasses.Update(gymClass);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private GymClassDto ToDto(GymClass c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Description = c.Description,
        StartTime = c.StartTime,
        EndTime = c.EndTime,
        Capacity = c.Capacity,
        BookedCount = c.BookedCount,
        AvailableSpots = c.AvailableSpots,
        InstructorName = c.InstructorName,
        Status = c.GetStatus(),
        TimeRange = c.GetTimeRange()
    };
}
