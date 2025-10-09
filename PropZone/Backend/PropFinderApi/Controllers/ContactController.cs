using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
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

        [HttpPost("lister")]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> ContactLister([FromBody] ContactListerRequestDto requestDto)
        {
            if(requestDto.PropertyId == Guid.Empty)
                throw new BadRequestException("Invalid property ID.");

            var buyerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _contactService.ContactListerAsync(requestDto, buyerId);
            return Ok(result);
        }

        [HttpGet("")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllContactLogs()
        {
            var logs = await _contactService.GetAllContactLogs();

            var response = _mapper.MapToOkResponse("Buyer Contact logs fetched successfully", logs);
            return Ok(response);
        }

        [HttpGet("logs/property/{propertyId:guid}")]
        [Authorize(Roles = "Lister,Admin")]
        public async Task<IActionResult> GetPropertyContactLogs(Guid propertyId)
        {
            var requesterId = GetUserId();
            var requesterRole = GetUserRole();

            var logs = await _contactService.GetContactLogsForPropertyAsync(propertyId, requesterId, requesterRole);

            var response = _mapper.MapToOkResponse("Property Contact logs fetched successfully", logs);
            return Ok(response);
        }

        [HttpGet("logs/lister/{listerId:guid}")]
        [Authorize(Roles = "Lister,Admin")]
        public async Task<IActionResult> GetListerContactLogs(Guid listerId)
        {
            var requesterId = GetUserId();
            var requesterRole = GetUserRole();

            var logs = await _contactService.GetContactLogsForListerAsync(listerId, requesterId, requesterRole);

            var response = _mapper.MapToOkResponse("Lister Contact logs fetched successfully", logs);
            return Ok(response);
        }


        [HttpGet("logs/buyer/{buyerId:guid}")]
        [Authorize(Roles = "Buyer,Admin")]
        public async Task<IActionResult> GetBuyerContactLogs(Guid buyerId)
        {
            var requesterId = GetUserId();
            var requesterRole = GetUserRole();

            var logs = await _contactService.GetContactLogsForBuyerAsync(buyerId, requesterId, requesterRole);

            var response =  _mapper.MapToOkResponse("All contact logs fetched successfully", logs);
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
