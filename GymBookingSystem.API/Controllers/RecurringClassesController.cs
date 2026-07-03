using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymBookingSystem.API.DTOs;
using GymBookingSystem.API.Services;

namespace GymBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecurringClassesController : ControllerBase
{
    private readonly IRecurringClassService _recurringClassService;
    private readonly ILogger<RecurringClassesController> _logger;

    public RecurringClassesController(IRecurringClassService recurringClassService, ILogger<RecurringClassesController> logger)
    {
        _recurringClassService = recurringClassService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<RecurringClassDto>>> GetRecurringClasses()
    {
        try
        {
            var result = await _recurringClassService.GetRecurringClassesAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recurring classes");
            throw;
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecurringClassDto>> GetRecurringClass(int id)
    {
        try
        {
            var result = await _recurringClassService.GetRecurringClassAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recurring class {Id}", id);
            throw;
        }
    }

    [Authorize(Roles = "Owner,Admin")]
    [HttpPost]
    public async Task<ActionResult<RecurringClassDto>> CreateRecurringClass([FromBody] CreateRecurringClassRequest request)
    {
        try
        {
            var result = await _recurringClassService.CreateRecurringClassAsync(request);
            return CreatedAtAction(nameof(GetRecurringClass), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating recurring class");
            throw;
        }
    }

    [Authorize(Roles = "Owner,Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<RecurringClassDto>> UpdateRecurringClass(int id, [FromBody] UpdateRecurringClassRequest request)
    {
        try
        {
            var result = await _recurringClassService.UpdateRecurringClassAsync(id, request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating recurring class {Id}", id);
            throw;
        }
    }

    [Authorize(Roles = "Owner,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecurringClass(int id)
    {
        try
        {
            await _recurringClassService.DeleteRecurringClassAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting recurring class {Id}", id);
            throw;
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("generate-instances")]
    public async Task<IActionResult> GenerateInstances([FromQuery] DateTime weekStart)
    {
        try
        {
            await _recurringClassService.GenerateInstancesForWeekAsync(weekStart);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating instances for week {WeekStart}", weekStart);
            throw;
        }
    }
}
