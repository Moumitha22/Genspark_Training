using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineAuctionAPI.Helpers;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class UserAccountStatusController : ControllerBase
{
    private readonly IUserAccountManageService _userAccountManageService;
    private readonly ILogger<UserAccountStatusController> _logger;

    public UserAccountStatusController(
        IUserAccountManageService userAccountManageService,
        ILogger<UserAccountStatusController> logger
    )
    {
        _userAccountManageService = userAccountManageService;
        _logger = logger;
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser([FromBody] UserDeleteRequest userDeleteDto)
    {
        await _userAccountManageService.SoftDeleteUserAsync(userDeleteDto);
        _logger.LogInformation("User with ID {UserId} deleted successfully", userDeleteDto.UserId);
        return Ok(ApiResponseHelper.CreateSuccess<string>(null, "User deleted successfully"));
    }

    [HttpPatch("restore")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RestoreUserByEmail([FromQuery] string email)
    {
        await _userAccountManageService.RestoreUserByEmailAsync(email);
        return Ok(ApiResponseHelper.CreateSuccess<string>(null, "User restored successfully"));
    }

    [HttpGet("get-delete-reason")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetDeleteReasonByEmail([FromQuery] string email)
    {
        var reason = await _userAccountManageService.GetDeleteReasonByEmail(email);
        if (reason == null)
            return NotFound(ApiResponseHelper.CreateNotFound<string>("Deletion reason not found"));
        return Ok(ApiResponseHelper.CreateSuccess(reason, "Deletion reason found successfully"));
    }

    [HttpGet("all-deleted-users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllDeletedUsers()
    {
        var deletedUsers = await _userAccountManageService.GetAllDeletedUsersAsync();
        if (deletedUsers == null)
            return NotFound(ApiResponseHelper.CreateNotFound<string>("No deleted users found"));
        return Ok(
            ApiResponseHelper.CreateSuccess(deletedUsers, "Deleted users retrieved successfully")
        );
    }
}
