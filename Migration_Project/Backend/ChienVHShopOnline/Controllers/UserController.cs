using System.Security.Claims;
using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // [HttpPost]
        // public async Task<ActionResult<UserResponseDto>> Create(UserRequestDto dto)
        // {
        //     var created = await _userService.CreateUser(dto);
        //     return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        // }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetById(int id)
        {
            var user = await _userService.GetUserById(id);
            return Ok(user);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized("Invalid user ID in token.");

            var user = await _userService.GetUserById(userId);
            return Ok(new { message = "User fetched successfully", data = user });
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponseDto>> Update(int id, UserUpdateDto dto)
        {
            var updated = await _userService.UpdateUser(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<UserResponseDto>> Delete(int id)
        {
            var deleted = await _userService.DeleteUser(id);
            return Ok(deleted);
        }
    }
}
