using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models.DTOs;
using System.Security.Claims;

namespace PropFinderApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class PropertyImageController : ControllerBase
    {
        private readonly IPropertyImageService _propertyImageService;
        private readonly IApiResponseMapper _responseMapper;


        public PropertyImageController(IPropertyImageService propertyImageService, IApiResponseMapper responseMapper)
        {
            _propertyImageService = propertyImageService;
            _responseMapper = responseMapper;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Lister,Admin")]
        public async Task<IActionResult> Upload([FromForm] PropertyImageUploadRequestDto imageUploadDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var file = imageUploadDto.File;
            if (file == null || file.Length == 0)
                throw new BadRequestException("Invalid file.");

            var image = await _propertyImageService.UploadImageAsync(imageUploadDto, userId);
            return Ok(_responseMapper.MapToOkResponse("Uploaded image successfully", image));
        }

        [HttpPost("upload-multiple")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Lister,Admin")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> UploadMultipleImages([FromForm] BulkPropertyImageUploadRequestDto dto)
        {
            var requesterId = GetUserId();
            var userRole = GetUserRole();
            var result = await _propertyImageService.UploadImagesAsync(dto, requesterId, userRole);

            return Ok(_responseMapper.MapToOkResponse("Uploaded images successfully", result));
        }

        [HttpGet("by-property/{propertyId:guid}")]
        public async Task<IActionResult> Get(Guid propertyId)
        {
            if (propertyId == Guid.Empty)
                throw new BadRequestException("Invalid property ID.");

            var images = await _propertyImageService.GetImagesByPropertyIdAsync(propertyId);
            return Ok(_responseMapper.MapToOkResponse("Images fetched successfully", images));
        }

        [Authorize(Roles = "Lister,Admin")]
        [HttpDelete("image/{id}")]
        public async Task<IActionResult> DeleteImage(Guid id)
        {
            var requesterId = GetUserId();
            var userRole = GetUserRole();
            await _propertyImageService.DeleteImageAsync(id, requesterId, userRole);
            return NoContent();
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



        
    