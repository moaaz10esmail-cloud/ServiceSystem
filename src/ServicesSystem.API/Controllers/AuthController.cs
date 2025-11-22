using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesSystem.Application.Interfaces;
using ServicesSystem.Shared.Common;
using ServicesSystem.Shared.DTOs.Auth;
using System.Security.Claims;

namespace ServicesSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<Result<AuthResponseDto>>> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<Result<AuthResponseDto>>> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<Result<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Change user password (requires authentication)
    /// </summary>
    [Authorize]
    [HttpPost("change-password")]
    public async Task<ActionResult<Result>> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(Result.Failure("Invalid user token"));
        }

        var result = await _authService.ChangePasswordAsync(userId, dto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Logout by revoking refresh token (requires authentication)
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<Result>> Logout([FromBody] RefreshTokenDto dto)
    {
        var result = await _authService.RevokeTokenAsync(dto.RefreshToken);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get current user information (requires authentication)
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public ActionResult<Result<object>> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var firstName = User.FindFirst("FirstName")?.Value;
        var lastName = User.FindFirst("LastName")?.Value;

        var userInfo = new
        {
            Id = userId,
            Email = email,
            Role = role,
            FirstName = firstName,
            LastName = lastName,
            FullName = $"{firstName} {lastName}"
        };

        return Ok(Result<object>.Success(userInfo, "User information retrieved successfully"));
    }
}
