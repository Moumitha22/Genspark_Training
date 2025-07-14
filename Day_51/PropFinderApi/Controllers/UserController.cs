using Microsoft.AspNetCore.Mvc; 
using PropFinderApi.Interfaces;
using PropFinderApi.Exceptions;
using Microsoft.AspNetCore.Authorization;
using PropFinderApi.Models.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.RateLimiting;

namespace PropFinderApi.Controllers
{
    [EnableRateLimiting("PerUserLimiter")]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IApiResponseMapper _responseMapper;

        public UserController(IUserService userService, IApiResponseMapper responseMapper)
        {
            _userService = userService;
            _responseMapper = responseMapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            var response = _responseMapper.MapToOkResponse("Users fetched successfully", users);
            return Ok(response);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _userService.GetUserByIdAsync(userId);
            var response = _responseMapper.MapToOkResponse("User fetched successfully", user);
            return Ok(response);
        }


        [HttpGet("{userId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);

            var response = _responseMapper.MapToOkResponse("User fetched successfully", user);
            return Ok(response);
        }

        [HttpGet("email/{email}")]
        [Authorize]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
                throw new NotFoundException($"No user found with email: {email}");

            var response = _responseMapper.MapToOkResponse("User fetched successfully", user);
            return Ok(response);
        }

        [HttpPut("{userId}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid userId,[FromBody] UserUpdateRequestDto dto)
        {
            var requesterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role)!;

            var updatedUser = await _userService.UpdateUserAsync(userId, dto, requesterId, userRole);
            var response = _responseMapper.MapToOkResponse("User updated successfully", updatedUser);
            return Ok(response);
        }


        [HttpPut("{userId:guid}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserStatusAsync(Guid userId,[FromQuery] bool disable)
        {
            if (userId == Guid.Empty)
                throw new BadRequestException("Invalid user ID.");

            await _userService.UpdateUserStatusAsync(userId, disable);
            var response = _responseMapper.MapToOkResponse("User status updated successfully");
            return Ok(response);
        }
    }
}
