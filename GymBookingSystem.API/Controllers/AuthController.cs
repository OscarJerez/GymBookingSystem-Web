using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymBookingSystem.API.Data;
using GymBookingSystem.API.Domain;
using GymBookingSystem.API.DTOs;
using GymBookingSystem.API.Services;

namespace GymBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly GymBookingDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthController(GymBookingDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            return BadRequest("Username already exists");

        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email already registered");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Member,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _tokenService.GenerateToken(user);
        return Ok(new AuthResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
            Token = token
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        if (!user.IsActive)
            return Unauthorized("Account is inactive");

        var token = _tokenService.GenerateToken(user);
        return Ok(new AuthResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
            Token = token
        });
    }
}
