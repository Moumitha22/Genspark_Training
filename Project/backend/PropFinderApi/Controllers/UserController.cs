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
        private readonly IPaginationService _paginationService;
        private readonly IApiResponseMapper _responseMapper;

        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, IPaginationService paginationService, IApiResponseMapper responseMapper, ILogger<UserController> logger)
        {
            _userService = userService;
            _paginationService = paginationService;
            _responseMapper = responseMapper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("This is a test log from /api/logtest/test endpoint.");
            _logger.LogInformation("Test log from controller");
            if (page <= 0 || pageSize <= 0)
                throw new BadRequestException("Page and pageSize must be greater than 0");

            var users = await _userService.GetAllUsersAsync();
            var (paginatedUsers, pagination) = _paginationService.ApplyPagination(users, page, pageSize);

            var response = _responseMapper.MapToOkResponse("Users fetched successfully", paginatedUsers, pagination);
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


        [HttpDelete("{userId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new BadRequestException("Invalid user ID.");

            await _userService.DeleteUserAsync(userId);
            var response = _responseMapper.MapToOkResponse("User deleted successfully");
            return Ok(response);
        }
    }
}
