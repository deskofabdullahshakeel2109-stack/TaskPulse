using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskPulse.Api.DTOs.Users;
using TaskPulse.Api.Helpers;
using TaskPulse.Api.Services.CurrentUser;
using TaskPulse.Api.Services.Users;

namespace TaskPulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(
    IUserService userService,
    ICurrentUserService currentUser) : ControllerBase
{
    // Only Admin can view all users.
    [HttpGet]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ActionResult<List<UserResponseDto>>> GetAll()
    {
        return Ok(await userService.GetAllAsync());
    }

    // Admin can view any user.
    // Normal users can only view themselves.
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponseDto>> GetById(Guid id)
    {
        if (!currentUser.IsAdmin && currentUser.UserId != id)
            return Forbid();

        var user = await userService.GetByIdAsync(id);

        return user is null
            ? NotFound(new { message = "User not found." })
            : Ok(user);
    }

    // Only Admin can search users by email.
    [HttpGet("email/{email}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ActionResult<UserResponseDto>> GetByEmail(string email)
    {
        var user = await userService.GetByEmailAsync(email);

        return user is null
            ? NotFound(new { message = "User not found." })
            : Ok(user);
    }

    // ONLY ADMIN CAN CREATE A NEW USER.
    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ActionResult<UserResponseDto>> Create(CreateUserDto dto)
    {
        try
        {
            var user = await userService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = user.Id },
                user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Admin can update any user.
    // Normal users can update themselves.
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserResponseDto>> Update(
        Guid id,
        UpdateUserDto dto)
    {
        if (!currentUser.IsAdmin && currentUser.UserId != id)
            return Forbid();

        try
        {
            var user = await userService.UpdateAsync(id, dto);

            return user is null
                ? NotFound(new { message = "User not found." })
                : Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Admin can delete any user.
    // Normal users can delete themselves.
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!currentUser.IsAdmin && currentUser.UserId != id)
            return Forbid();

        var deleted = await userService.DeleteAsync(id);

        return deleted
            ? Ok(new { message = "User deleted successfully." })
            : NotFound(new { message = "User not found." });
    }
}