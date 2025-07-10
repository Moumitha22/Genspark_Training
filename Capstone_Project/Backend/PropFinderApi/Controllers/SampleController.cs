using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace PropFinderApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SampleController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public IActionResult Greet()
        {
            var userId = GetUserId();
            var role = GetUserRole();
            return Ok(new { message = $"Hi {role}, your ID is {userId}" });
        }

         private Guid GetUserId()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return userId;
        }

        private string GetUserRole()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            return role;
        }
    }
}
