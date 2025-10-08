using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using System.Security.Claims;

namespace PropFinderApi.Controllers
{
    [EnableRateLimiting("PerUserLimiter")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ContactController : ControllerBase
    {
        private readonly IContactLogService _contactService;
        private readonly IPaginationService _paginationService;
        private readonly IApiResponseMapper _mapper;

        public ContactController(IContactLogService contactService, IPaginationService paginationService, IApiResponseMapper mapper)
        {
            _contactService = contactService;
            _paginationService = paginationService;
            _mapper = mapper;
        }

        [HttpPost("agent")]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> ContactAgent([FromBody] ContactAgentRequestDto requestDto)
        {
            if(requestDto.PropertyId == Guid.Empty)
                throw new BadRequestException("Invalid property ID.");

            var buyerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _contactService.ContactAgentAsync(requestDto, buyerId);
            return Ok(result);
        }

        [HttpGet("")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllContactLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                throw new BadRequestException("Page and pageSize must be greater than 0");

            var logs = await _contactService.GetAllContactLogs();
            var (paginatedLogs, pagination) = _paginationService.ApplyPagination(logs, page, pageSize);

            var response = _mapper.MapToOkResponse("Contact logs fetched successfully", paginatedLogs, pagination);
            return Ok(response);
        }

        [HttpGet("logs/agent/{agentId:guid}")]
        [Authorize(Roles = "Agent,Admin")]
        public async Task<IActionResult> GetAgentContactLogs(Guid agentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                throw new BadRequestException("Page and pageSize must be greater than 0");

            var requesterId = GetUserId();
            var requesterRole = GetUserRole();

            var logs = await _contactService.GetContactLogsForAgentAsync(agentId, requesterId, requesterRole);
            var (paginatedLogs, pagination) = _paginationService.ApplyPagination(logs, page, pageSize);

            var response = _mapper.MapToOkResponse("Contact logs fetched successfully", paginatedLogs, pagination);
            return Ok(response);
        }


        [HttpGet("logs/buyer/{buyerId:guid}")]
        [Authorize(Roles = "Buyer,Admin")]
        public async Task<IActionResult> GetBuyerContactLogs(Guid buyerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                throw new BadRequestException("Page and pageSize must be greater than 0");

            var requesterId = GetUserId();
            var requesterRole = GetUserRole();

            var logs = await _contactService.GetContactLogsForBuyerAsync(buyerId, requesterId, requesterRole);

            var (paginatedLogs, pagination) = _paginationService.ApplyPagination(logs, page, pageSize);

            var response =  _mapper.MapToOkResponse("All contact logs fetched successfully", paginatedLogs, pagination);
            return Ok(response);
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
