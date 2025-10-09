using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropFinderApi.Interfaces;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Controllers
{
    [ApiController]
    [Route("api/v1/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AdminDashboardDto>> GetAdminDashboard()
        {
            var result = await _dashboardService.GetAdminDashboardAsync();
            return Ok(new { data = result });
        }

        [HttpGet("lister")]
        [Authorize(Roles = "Lister")]
        public async Task<ActionResult<ListerDashboardDto>> GetListerDashboard()
        {
            var listerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _dashboardService.GetListerDashboardAsync(listerId);
            return Ok(new { data = result });
        }
    }

    
}