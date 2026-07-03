using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GymBookingSystem.API.DTOs;
using GymBookingSystem.API.Services;

namespace GymBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpGet("membership-plans")]
    public async Task<ActionResult<MembershipPlanDto[]>> GetMembershipPlans()
    {
        try
        {
            var plans = await _paymentService.GetMembershipPlansAsync();
            return Ok(plans);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving membership plans");
            throw;
        }
    }

    [Authorize]
    [HttpPost("memberships")]
    public async Task<ActionResult<MembershipDto>> CreateMembership([FromBody] CreateMembershipRequest request)
    {
        try
        {
            var userId = GetUserId();
            var result = await _paymentService.CreateMembershipAsync(userId, request.Type);
            return CreatedAtAction(nameof(GetActiveMembership), result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating membership for user {UserId}", GetUserId());
            throw;
        }
    }

    [Authorize]
    [HttpGet("active-membership")]
    public async Task<ActionResult<MembershipDto>> GetActiveMembership()
    {
        try
        {
            var userId = GetUserId();
            var membership = await _paymentService.GetActiveMembershipAsync(userId);
            if (membership == null)
                return NoContent();
            return Ok(membership);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active membership for user {UserId}", GetUserId());
            throw;
        }
    }

    [Authorize]
    [HttpPost("process")]
    public async Task<ActionResult<PaymentDto>> ProcessPayment([FromBody] CreatePaymentRequest request)
    {
        try
        {
            var userId = GetUserId();
            var result = await _paymentService.ProcessPaymentAsync(userId, request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for user {UserId}", GetUserId());
            throw;
        }
    }

    [Authorize]
    [HttpGet("history")]
    public async Task<ActionResult<List<PaymentDto>>> GetPaymentHistory()
    {
        try
        {
            var userId = GetUserId();
            var result = await _paymentService.GetPaymentHistoryAsync(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment history for user {UserId}", GetUserId());
            throw;
        }
    }
}
