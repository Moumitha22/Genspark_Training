using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PropFinderApi.Controllers
{
    [EnableRateLimiting("PerUserLimiter")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class FeatureMasterController : ControllerBase
    {
        private readonly IFeatureMasterService _featureMasterService;
        private readonly IApiResponseMapper _mapper;

        public FeatureMasterController(IFeatureMasterService featureService, IApiResponseMapper mapper)
        {
            _featureMasterService = featureService;
            _mapper = mapper;
        }

        [HttpPost("feature")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFeature([FromBody] FeatureMasterAddRequestDto dto)
        {
            var result = await _featureMasterService.CreateFeatureAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllFeatures()
        {
            var features = await _featureMasterService.GetAllFeaturesAsync();

            return Ok(new ApiResponse<IEnumerable<FeatureAdminDto>>
            {
                Success = true,
                Message = "Applicable features fetched successfully",
                Data = features
            });
        }

        [HttpGet("applicable")]
        public async Task<IActionResult> GetApplicableFeatures(
            [FromQuery] ListingPurpose listingPurpose,
            [FromQuery] PropertyType? propertyType = null)
        {
            IEnumerable<FeatureFieldDto> features;

            if (propertyType.HasValue)
            {
                features = await _featureMasterService.GetApplicableFeaturesAsync(propertyType.Value, listingPurpose);
            }
            else
            {
                features = await _featureMasterService.GetApplicableFeaturesByPurposeAsync(listingPurpose);
            }

            return Ok(new ApiResponse<IEnumerable<FeatureFieldDto>>
            {
                Success = true,
                Message = "Applicable features fetched successfully",
                Data = features
            });
        }

        [HttpPut("feature/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFeature([FromRoute] Guid id, [FromBody] FeatureMasterAddRequestDto dto)
        {
            var updated = await _featureMasterService.UpdateFeatureAsync(id, dto);
            return Ok(new ApiResponse<FeatureFieldDto>
            {
                Success = true,
                Message = "Feature updated successfully",
                Data = updated
            });
        }


        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFeature(Guid id)
        {
            System.Console.WriteLine("Delete feature :"+id);
            if (id == Guid.Empty)
                throw new BadRequestException("Invalid feature ID.");

            await _featureMasterService.SoftDeleteFeatureAsync(id);

            var response = _mapper.MapToOkResponse("Feature deleted successfully");
            return Ok(response);
        }

    }
}
