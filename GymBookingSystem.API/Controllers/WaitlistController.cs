using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GymBookingSystem.API.DTOs;
using GymBookingSystem.API.Services;

namespace GymBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WaitlistController : ControllerBase
{
    private readonly IWaitlistService _waitlistService;
    private readonly ILogger<WaitlistController> _logger;

    public WaitlistController(IWaitlistService waitlistService, ILogger<WaitlistController> logger)
    {
        _waitlistService = waitlistService;
        _logger = logger;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpPost]
    public async Task<ActionResult<WaitlistDto>> JoinWaitlist([FromBody] JoinWaitlistRequest request)
    {
        try
        {
            var userId = GetUserId();
            var result = await _waitlistService.JoinWaitlistAsync(userId, request.ClassId);
            return CreatedAtAction(nameof(GetMyWaitlist), result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining waitlist for class {ClassId}", request.ClassId);
            throw;
        }
    }

    [HttpGet("my-waitlist")]
    public async Task<ActionResult<List<WaitlistDto>>> GetMyWaitlist()
    {
        try
        {
            var userId = GetUserId();
            var result = await _waitlistService.GetMyWaitlistAsync(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving waitlist for user {UserId}", GetUserId());
            throw;
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromWaitlist(int id)
    {
        try
        {
            var userId = GetUserId();
            await _waitlistService.RemoveFromWaitlistAsync(id, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing from waitlist {WaitlistId}", id);
            throw;
        }
    }
}
