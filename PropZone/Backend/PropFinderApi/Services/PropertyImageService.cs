using Microsoft.AspNetCore.StaticFiles;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Services
{
    public class PropertyImageService : IPropertyImageService
    {
        private readonly IRepository<Guid, PropertyImage> _imageRepository;
        private readonly IRepository<Guid, Property> _propertyRepository;

        private readonly IWebHostEnvironment _env;

        public PropertyImageService(IRepository<Guid, PropertyImage> imageRepository, IRepository<Guid, Property> propertyRepository, IWebHostEnvironment env)
        {
            _imageRepository = imageRepository;
            _propertyRepository = propertyRepository;
            _env = env;
        }

        public async Task<PropertyImage> UploadImageAsync(PropertyImageUploadRequestDto imageUploadDto, Guid requesterId)
        {
            Property? property = null;
            try
            {
                property = await _propertyRepository.Get(imageUploadDto.PropertyId);
            }
            catch (NotFoundException)
            {

            }

            if (property != null && property.ListerId != requesterId)
                throw new UnauthorizedException("You can only upload images for your own properties.");

            var uploadsPath = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageUploadDto.File.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageUploadDto.File.CopyToAsync(stream);
            }

            var image = new PropertyImage
            {
                Id = Guid.NewGuid(),
                PropertyId = imageUploadDto.PropertyId,
                ImageUrl = $"/images/{fileName}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            return await _imageRepository.Add(image);
        }

        public async Task<IEnumerable<PropertyImage>> UploadImagesAsync(BulkPropertyImageUploadRequestDto dto, Guid requesterId, string userRole)
        {
            var property = await _propertyRepository.Get(dto.PropertyId);

            if (userRole == "Lister" && property.ListerId != requesterId)
                throw new UnauthorizedException("You can only upload images for your own properties.");

            var uploadsPath = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var uploadedImages = new List<PropertyImage>();

            foreach (var file in dto.Files)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                var image = new PropertyImage
                {
                    Id = Guid.NewGuid(),
                    PropertyId = dto.PropertyId,
                    ImageUrl = $"/images/{fileName}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _imageRepository.Add(image);
                uploadedImages.Add(image);
            }

            return uploadedImages;
        }


        public async Task<IEnumerable<PropertyImage>> GetImagesByPropertyIdAsync(Guid propertyId)
        {
            var all = await _imageRepository.GetAll();
            return all.Where(i => i.PropertyId == propertyId);
        }

        public async Task DeleteImageAsync(Guid imageId, Guid requesterId, string userRole)
        {
            var image = await _imageRepository.Get(imageId);
            var property = await _propertyRepository.Get(image.PropertyId);
            
            if (userRole == "Lister" && property.ListerId != requesterId)
                throw new UnauthorizedException("You can't delete images for other users' properties.");

            var filePath = Path.Combine(_env.WebRootPath, image.ImageUrl.TrimStart('/'));
            if (File.Exists(filePath))
                File.Delete(filePath);

            await _imageRepository.Delete(imageId);
        }

    }
   
}