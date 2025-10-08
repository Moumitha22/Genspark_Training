using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Interfaces
{
    public interface IPropertyImageService
    {
        Task<PropertyImage> UploadImageAsync(PropertyImageUploadDto imageUploadDto, Guid requesterId);
        Task<IEnumerable<PropertyImage>> GetImagesByPropertyIdAsync(Guid propertyId);
        Task<(byte[] fileContent, string contentType)> GetImageContentByIdAsync(Guid imageId);
    }
}