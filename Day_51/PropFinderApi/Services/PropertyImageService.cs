// using Microsoft.AspNetCore.StaticFiles;
// using PropFinderApi.Exceptions;
// using PropFinderApi.Interfaces;
// using PropFinderApi.Models;
// using PropFinderApi.Models.DTOs;

// namespace PropFinderApi.Services
// {
//     public class PropertyImageService : IPropertyImageService
//     {
//         private readonly IRepository<Guid, PropertyImage> _imageRepository;
//         private readonly IRepository<Guid, Property> _propertyRepository;

//         private readonly IBlobStorageService _blobStorageService;

//         private readonly IWebHostEnvironment _env;

//         public PropertyImageService(IRepository<Guid, PropertyImage> imageRepository, IRepository<Guid, Property> propertyRepository, IWebHostEnvironment env, IBlobStorageService blobStorageService)
//         {
//             _imageRepository = imageRepository;
//             _propertyRepository = propertyRepository;
//             _blobStorageService = blobStorageService;
//             _env = env;
//         }

//         public async Task<PropertyImage> UploadImageAsync(PropertyImageUploadRequestDto imageUploadDto, Guid requesterId)
//         {
//             Property? property = null;
//             try
//             {
//                 property = await _propertyRepository.Get(imageUploadDto.PropertyId);
//             }
//             catch (NotFoundException)
//             {
//                 // Handle property not found
//             }

//             if (property == null || property.ListerId != requesterId)
//                 throw new UnauthorizedException("You can only upload images for your own properties.");

//             var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageUploadDto.File.FileName)}";
//             var blobPath = $"{fileName}";

//             using var stream = imageUploadDto.File.OpenReadStream();
//             await _blobStorageService.UploadFileToPathAsync(stream, blobPath);
//             var imageUrl = _blobStorageService.GetBlobUrl(blobPath);

//             var image = new PropertyImage
//             {
//                 Id = Guid.NewGuid(),
//                 PropertyId = imageUploadDto.PropertyId,
//                 ImageUrl = imageUrl,
//                 CreatedAt = DateTime.UtcNow,
//                 UpdatedAt = DateTime.UtcNow,
//                 IsDeleted = false
//             };

//             return await _imageRepository.Add(image);
//         }

//         public async Task<IEnumerable<PropertyImage>> UploadImagesAsync(BulkPropertyImageUploadRequestDto dto, Guid requesterId, string userRole)
//         {
//             var property = await _propertyRepository.Get(dto.PropertyId);

//             if (userRole == "Lister" && property.ListerId != requesterId)
//                 throw new UnauthorizedException("You can only upload images for your own properties.");

//             var uploadedImages = new List<PropertyImage>();

//             foreach (var file in dto.Files)
//             {
//                 var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
//                 var blobPath = fileName;

//                 using var stream = file.OpenReadStream();
//                 await _blobStorageService.UploadFileToPathAsync(stream, blobPath);
//                 var imageUrl = _blobStorageService.GetBlobUrl(blobPath);

//                 var image = new PropertyImage
//                 {
//                     Id = Guid.NewGuid(),
//                     PropertyId = dto.PropertyId,
//                     ImageUrl = imageUrl,
//                     CreatedAt = DateTime.UtcNow,
//                     UpdatedAt = DateTime.UtcNow,
//                     IsDeleted = false
//                 };

//                 await _imageRepository.Add(image);
//                 uploadedImages.Add(image);
//             }

//             return uploadedImages;
//         }

//         // --- Legacy local file upload (retained) ---

//         // public async Task<PropertyImage> UploadImageAsync(PropertyImageUploadRequestDto imageUploadDto, Guid requesterId)
//         // {
//         //     Property? property = null;
//         //     try
//         //     {
//         //         property = await _propertyRepository.Get(imageUploadDto.PropertyId);
//         //     }
//         //     catch (NotFoundException)
//         //     {
//         //     }

//         //     if (property != null && property.ListerId != requesterId)
//         //         throw new UnauthorizedException("You can only upload images for your own properties.");

//         //     var uploadsPath = Path.Combine(_env.WebRootPath, "images");
//         //     if (!Directory.Exists(uploadsPath))
//         //         Directory.CreateDirectory(uploadsPath);

//         //     var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageUploadDto.File.FileName)}";
//         //     var filePath = Path.Combine(uploadsPath, fileName);

//         //     using (var stream = new FileStream(filePath, FileMode.Create))
//         //     {
//         //         await imageUploadDto.File.CopyToAsync(stream);
//         //     }

//         //     var image = new PropertyImage
//         //     {
//         //         Id = Guid.NewGuid(),
//         //         PropertyId = imageUploadDto.PropertyId,
//         //         ImageUrl = $"/images/{fileName}",
//         //         CreatedAt = DateTime.UtcNow,
//         //         UpdatedAt = DateTime.UtcNow,
//         //         IsDeleted = false
//         //     };

//         //     return await _imageRepository.Add(image);
//         // }

//         // public async Task<IEnumerable<PropertyImage>> UploadImagesAsync(BulkPropertyImageUploadRequestDto dto, Guid requesterId, string userRole)
//         // {
//         //     var property = await _propertyRepository.Get(dto.PropertyId);

//         //     if (userRole == "Lister" && property.ListerId != requesterId)
//         //         throw new UnauthorizedException("You can only upload images for your own properties.");

//         //     var uploadsPath = Path.Combine(_env.WebRootPath, "images");
//         //     if (!Directory.Exists(uploadsPath))
//         //         Directory.CreateDirectory(uploadsPath);

//         //     var uploadedImages = new List<PropertyImage>();

//         //     foreach (var file in dto.Files)
//         //     {
//         //         var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
//         //         var filePath = Path.Combine(uploadsPath, fileName);

//         //         using var stream = new FileStream(filePath, FileMode.Create);
//         //         await file.CopyToAsync(stream);

//         //         var image = new PropertyImage
//         //         {
//         //             Id = Guid.NewGuid(),
//         //             PropertyId = dto.PropertyId,
//         //             ImageUrl = $"/images/{fileName}",
//         //             CreatedAt = DateTime.UtcNow,
//         //             UpdatedAt = DateTime.UtcNow,
//         //             IsDeleted = false
//         //         };

//         //         await _imageRepository.Add(image);
//         //         uploadedImages.Add(image);
//         //     }

//         //     return uploadedImages;
//         // }

//         public async Task<IEnumerable<PropertyImage>> GetImagesByPropertyIdAsync(Guid propertyId)
//         {
//             var all = await _imageRepository.GetAll();
//             return all.Where(i => i.PropertyId == propertyId);
//         }

//         public async Task DeleteImageAsync(Guid imageId, Guid requesterId, string userRole)
//         {
//             var image = await _imageRepository.Get(imageId);
//             var property = await _propertyRepository.Get(image.PropertyId);

//             if (userRole == "Lister" && property.ListerId != requesterId)
//                 throw new UnauthorizedException("You can't delete images for other users' properties.");

//             if (image.ImageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
//             {
//                 var basePath = await _blobStorageService.GetContainerBaseUrlAsync();
//                 var blobPath = image.ImageUrl.Replace(basePath, "").Split('?')[0].TrimStart('/');
//                 await _blobStorageService.DeleteFileAsync(blobPath);
//             }
//             else
//             {
//                 var filePath = Path.Combine(_env.WebRootPath, image.ImageUrl.TrimStart('/'));
//                 if (File.Exists(filePath))
//                     File.Delete(filePath);
//             }

//             await _imageRepository.Delete(imageId);
//         }

//         public async Task<(byte[] fileContent, string contentType)> GetImageContentByIdAsync(Guid imageId)
//         {
//             var image = await _imageRepository.Get(imageId);

//             if (image.ImageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
//             {
//                 var basePath = await _blobStorageService.GetContainerBaseUrlAsync();
//                 var blobPath = image.ImageUrl.Replace(basePath, "").Split('?')[0].TrimStart('/');
//                 var stream = await _blobStorageService.DownloadFileAsync(blobPath);

//                 using var memoryStream = new MemoryStream();
//                 await stream.CopyToAsync(memoryStream);
//                 var contentType = GetContentType(blobPath);
//                 return (memoryStream.ToArray(), contentType);
//             }
//             else
//             {
//                 var filePath = Path.Combine(_env.WebRootPath, image.ImageUrl.TrimStart('/'));

//                 if (!File.Exists(filePath))
//                     throw new NotFoundException("Image file not found on server");

//                 var fileContent = await File.ReadAllBytesAsync(filePath);
//                 var contentType = GetContentType(filePath);

//                 return (fileContent, contentType);
//             }
//         }

//         private string GetContentType(string filePath)
//         {
//             var provider = new FileExtensionContentTypeProvider();
//             if (!provider.TryGetContentType(filePath, out var contentType))
//                 contentType = "application/octet-stream";
//             return contentType;
//         }
//     }
// }


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
        private readonly IBlobStorageService _blobStorageService;
        private readonly IWebHostEnvironment _env;

        public PropertyImageService(
            IRepository<Guid, PropertyImage> imageRepository,
            IRepository<Guid, Property> propertyRepository,
            IWebHostEnvironment env,
            IBlobStorageService blobStorageService)
        {
            _imageRepository = imageRepository;
            _propertyRepository = propertyRepository;
            _blobStorageService = blobStorageService;
            _env = env;
        }

        public async Task<PropertyImage> UploadImageAsync(PropertyImageUploadRequestDto imageUploadDto, Guid requesterId)
        {
            Property? property = null;
            try
            {
                property = await _propertyRepository.Get(imageUploadDto.PropertyId);
            }
            catch (NotFoundException) { }

            if (property == null || property.ListerId != requesterId)
                throw new UnauthorizedException("You can only upload images for your own properties.");

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageUploadDto.File.FileName)}";
            var blobPath = fileName;

            using var stream = imageUploadDto.File.OpenReadStream();
            await _blobStorageService.UploadFileToPathAsync(stream, blobPath);
            var imageUrl = await _blobStorageService.GetBlobUrlAsync(blobPath);

            var image = new PropertyImage
            {
                Id = Guid.NewGuid(),
                PropertyId = imageUploadDto.PropertyId,
                ImageUrl = imageUrl,
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

            var uploadedImages = new List<PropertyImage>();

            foreach (var file in dto.Files)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var blobPath = fileName;

                using var stream = file.OpenReadStream();
                await _blobStorageService.UploadFileToPathAsync(stream, blobPath);
                var imageUrl = await _blobStorageService.GetBlobUrlAsync(blobPath);

                var image = new PropertyImage
                {
                    Id = Guid.NewGuid(),
                    PropertyId = dto.PropertyId,
                    ImageUrl = imageUrl,
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

            if (image.ImageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                var basePath = await _blobStorageService.GetContainerBaseUrlAsync();
                var blobPath = image.ImageUrl.Replace(basePath, "").Split('?')[0].TrimStart('/');
                await _blobStorageService.DeleteFileAsync(blobPath);
            }
            else
            {
                var filePath = Path.Combine(_env.WebRootPath, image.ImageUrl.TrimStart('/'));
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            await _imageRepository.Delete(imageId);
        }

        public async Task<(byte[] fileContent, string contentType)> GetImageContentByIdAsync(Guid imageId)
        {
            var image = await _imageRepository.Get(imageId);

            var filePath = Path.Combine(_env.WebRootPath, image.ImageUrl.TrimStart('/'));

            if (!File.Exists(filePath))
                throw new NotFoundException("Image file not found on server");

            var fileContent = await File.ReadAllBytesAsync(filePath);
            var contentType = GetContentType(filePath);

            return (fileContent, contentType);
        }

        private string GetContentType(string filePath)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
                contentType = "application/octet-stream";
            return contentType;
        }
    }
}
