using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskPulse.Api.DTOs.Auth;
using TaskPulse.Api.Helpers;
using TaskPulse.Api.Services.Auth;
using TaskPulse.Api.Services.CurrentUser;

namespace TaskPulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IAuthService authService,
    ICurrentUserService currentUserService) : ControllerBase
{
    // Only Admin users can create/register new users.
    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto dto)
    {
        try
        {
            return Ok(await authService.RegisterAsync(dto));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // Anyone with an existing account can log in.
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDto dto)
    {
        var result = await authService.LoginAsync(dto);

        return result is null
            ? Unauthorized(new { message = "Invalid email or password." })
            : Ok(result);
    }

    // Any authenticated user can view their own profile.
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult> Me()
    {
        if (currentUserService.UserId is not Guid userId)
            return Unauthorized();

        var result = await authService.GetMeAsync(userId);

        return result is null
            ? NotFound(new { message = "User not found." })
            : Ok(result);
    }
}