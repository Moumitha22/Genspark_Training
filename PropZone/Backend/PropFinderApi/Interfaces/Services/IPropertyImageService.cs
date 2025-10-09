using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Interfaces
{
    public interface IPropertyImageService
    {
        Task<PropertyImage> UploadImageAsync(PropertyImageUploadRequestDto imageUploadDto, Guid requesterId);
        Task<IEnumerable<PropertyImage>> UploadImagesAsync(BulkPropertyImageUploadRequestDto dto, Guid requesterId, string userRole);
        Task DeleteImageAsync(Guid imageId, Guid requesterId, string userRole);
        Task<IEnumerable<PropertyImage>> GetImagesByPropertyIdAsync(Guid propertyId);
    }
}