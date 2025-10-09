using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropFinderApi.Interfaces;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DiscountCodeController : ControllerBase
    {
        private readonly IDiscountCodeService _discountCodeService;
        private readonly IApiResponseMapper _apiResponseMapper;

        public DiscountCodeController(
            IDiscountCodeService discountCodeService,
            IApiResponseMapper apiResponseMapper
        )
        {
            _discountCodeService = discountCodeService;
            _apiResponseMapper = apiResponseMapper;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DiscountCodeDto>> CreateDiscountCode(
            DiscountCodeAddRequestDto request
        )
        {
            var result = await _discountCodeService.CreateDiscountCodeAsync(request);
            return Ok(
                _apiResponseMapper.MapToOkResponse("Discount code created successfully", result)
            );
        }

        [HttpGet("active")]
        [Authorize(Roles = "Lister,Buyer, Admin")]
        public async Task<ActionResult<IEnumerable<DiscountCodeDto>>> GetActiveDiscountCodes(
            [FromQuery] ActiveDiscountCodeFilterRequestDto filterRequestDto
        )
        {
            var result = await _discountCodeService.GetActiveDiscountCodesAsync(filterRequestDto);
            return Ok(new { data = result });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DiscountCodeDto>> GetDiscountCodeById(Guid id)
        {
            var result = await _discountCodeService.GetDiscountCodeByIdAsync(id);
            return Ok(
                _apiResponseMapper.MapToOkResponse("Discount code retrieved successfully", result)
            );
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SearchDiscountCodes(
            [FromQuery] BasicDiscountFilterModel filterRequest,
            [FromQuery] SortModel sortModel,
            [FromQuery] PaginationModel paginationModel
        )
        {
            var result = await _discountCodeService.SearchDiscountCodesAsync(
                filterRequest,
                sortModel,
                paginationModel
            );

            return Ok(
                _apiResponseMapper.MapToOkResponse("Discount codes retrieved successfully", result)
            );
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DiscountCodeDto>> UpdateDiscountCode(
            Guid id,
            DiscountCodeUpdateRequestDto request
        )
        {
            var result = await _discountCodeService.UpdateDiscountCodeAsync(id, request);
            return Ok(
                _apiResponseMapper.MapToOkResponse("Discount code updated successfully", result)
            );
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteDiscountCode(Guid id, [FromQuery] bool disable)
        {
            var result = await _discountCodeService.UpdateDiscountDeletion(id, disable);
            if (result)
            {
                return Ok(
                    _apiResponseMapper.MapToOkResponse(
                        "Discount code deleted status updated successfully",
                        true
                    )
                );
            }
            return NotFound(
                _apiResponseMapper.MapToErrorResponse<bool>("Discount code not found", false)
            );
        }

        [HttpPost("simulateDiscount")]
        public async Task<IActionResult> SimulateDiscount([FromBody] DiscountSimulationRequest dto)
        {
            var result = await _discountCodeService.SimulateDiscountAsync(dto);
            return Ok(
                _apiResponseMapper.MapToOkResponse(
                    "Discount simulation applied successfully",
                    result
                )
            );
        }
    }
}
