using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PropFinderApi.Services;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Interfaces;
using PropFinderApi.Exceptions;
using System.Text;
using System.IO;

namespace PropFinderApi.Tests.Services
{
    public class PropertyImageServiceTests
    {
        private Mock<IRepository<Guid, PropertyImage>> _imageRepoMock = null!;
        private Mock<IRepository<Guid, Property>> _propertyRepoMock = null!;
        private Mock<IWebHostEnvironment> _envMock = null!;
        private PropertyImageService _service = null!;
        private string _testWebRootPath = null!;

        [SetUp]
        public void SetUp()
        {
            _imageRepoMock = new Mock<IRepository<Guid, PropertyImage>>();
            _propertyRepoMock = new Mock<IRepository<Guid, Property>>();
            _envMock = new Mock<IWebHostEnvironment>();

            _testWebRootPath = Path.Combine(Path.GetTempPath(), "webroot_test");
            Directory.CreateDirectory(_testWebRootPath);
            _envMock.Setup(e => e.WebRootPath).Returns(_testWebRootPath);

            _service = new PropertyImageService(_imageRepoMock.Object, _propertyRepoMock.Object, _envMock.Object);
        }

        [Test]
        public async Task UploadImageAsync_ValidLister_UploadsSuccessfully()
        {
            var listerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            var property = new Property { Id = propertyId, ListerId = listerId };

            var fileMock = new Mock<IFormFile>();
            var content = "dummy image data";
            var fileName = "test.jpg";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns((Stream stream, CancellationToken _) =>
            {
                return ms.CopyToAsync(stream);
            });

            _propertyRepoMock.Setup(r => r.Get(propertyId)).ReturnsAsync(property);
            _imageRepoMock.Setup(r => r.Add(It.IsAny<PropertyImage>())).ReturnsAsync((PropertyImage img) => img);

            var dto = new PropertyImageUploadRequestDto { PropertyId = propertyId, File = fileMock.Object };

            var result = await _service.UploadImageAsync(dto, listerId);

            Assert.IsNotNull(result);
            Assert.That(result.ImageUrl, Does.StartWith("/images/"));
            Assert.That(File.Exists(Path.Combine(_testWebRootPath, result.ImageUrl.TrimStart('/'))));
        }

        [Test]
        public void UploadImageAsync_InvalidLister_ThrowsUnauthorized()
        {
            var requesterId = Guid.NewGuid();
            var property = new Property { Id = Guid.NewGuid(), ListerId = Guid.NewGuid() };

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.jpg");

            _propertyRepoMock.Setup(r => r.Get(property.Id)).ReturnsAsync(property);

            var dto = new PropertyImageUploadRequestDto { PropertyId = property.Id, File = fileMock.Object };

            Assert.ThrowsAsync<UnauthorizedException>(async () =>
            {
                await _service.UploadImageAsync(dto, requesterId);
            });
        }

        [Test]
        public async Task UploadImagesAsync_ValidLister_UploadsAllFiles()
        {
            var listerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            var property = new Property { Id = propertyId, ListerId = listerId };

            var files = new List<IFormFile>();

            for (int i = 0; i < 3; i++)
            {
                var fileMock = new Mock<IFormFile>();
                var content = $"dummy image {i}";
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
                fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
                fileMock.Setup(f => f.FileName).Returns($"image{i}.jpg");
                fileMock.Setup(f => f.Length).Returns(ms.Length);
                fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns((Stream stream, CancellationToken _) => ms.CopyToAsync(stream));
                files.Add(fileMock.Object);
            }

            _propertyRepoMock.Setup(r => r.Get(propertyId)).ReturnsAsync(property);
            _imageRepoMock.Setup(r => r.Add(It.IsAny<PropertyImage>())).ReturnsAsync((PropertyImage img) => img);

            var dto = new BulkPropertyImageUploadRequestDto { PropertyId = propertyId, Files = files };

            var result = await _service.UploadImagesAsync(dto, listerId, "Lister");

            Assert.That(result.Count(), Is.EqualTo(3));
            foreach (var img in result)
            {
                Assert.That(img.ImageUrl, Does.StartWith("/images/"));
                Assert.That(File.Exists(Path.Combine(_testWebRootPath, img.ImageUrl.TrimStart('/'))));
            }
        }

        [Test]
        public void UploadImagesAsync_InvalidLister_ThrowsUnauthorized()
        {
            var requesterId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            var property = new Property { Id = propertyId, ListerId = Guid.NewGuid() };

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("image.jpg");

            var dto = new BulkPropertyImageUploadRequestDto
            {
                PropertyId = propertyId,
                Files = new List<IFormFile> { fileMock.Object }
            };

            _propertyRepoMock.Setup(r => r.Get(propertyId)).ReturnsAsync(property);

            Assert.ThrowsAsync<UnauthorizedException>(async () =>
            {
                await _service.UploadImagesAsync(dto, requesterId, "Lister");
            });
        }

        [Test]
        public async Task GetImagesByPropertyIdAsync_ReturnsFilteredImages()
        {
            var propId = Guid.NewGuid();
            var otherPropId = Guid.NewGuid();

            var images = new List<PropertyImage>
        {
            new() { Id = Guid.NewGuid(), PropertyId = propId },
            new() { Id = Guid.NewGuid(), PropertyId = otherPropId }
        };

            _imageRepoMock.Setup(r => r.GetAll()).ReturnsAsync(images);

            var result = await _service.GetImagesByPropertyIdAsync(propId);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().PropertyId, Is.EqualTo(propId));
        }

        [Test]
        public async Task DeleteImageAsync_ValidLister_DeletesImageAndFile()
        {
            var listerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            var imageId = Guid.NewGuid();

            var fileName = "to_delete.jpg";
            var filePath = Path.Combine(_testWebRootPath, "images", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            await File.WriteAllTextAsync(filePath, "fake image");

            var image = new PropertyImage { Id = imageId, PropertyId = propertyId, ImageUrl = "/images/" + fileName };
            var property = new Property { Id = propertyId, ListerId = listerId };

            _imageRepoMock.Setup(r => r.Get(imageId)).ReturnsAsync(image);
            _propertyRepoMock.Setup(r => r.Get(propertyId)).ReturnsAsync(property);
            _imageRepoMock.Setup(r => r.Get(imageId)).ReturnsAsync(image);

            await _service.DeleteImageAsync(imageId, listerId, "Lister");

            Assert.That(File.Exists(filePath), Is.False);
            _imageRepoMock.Verify(r => r.Delete(imageId), Times.Once);
        }

        [Test]
        public void DeleteImageAsync_InvalidLister_ThrowsUnauthorized()
        {
            var imageId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();

            var image = new PropertyImage { Id = imageId, PropertyId = propertyId, ImageUrl = "/images/image.jpg" };
            var property = new Property { Id = propertyId, ListerId = Guid.NewGuid() }; // different lister

            _imageRepoMock.Setup(r => r.Get(imageId)).ReturnsAsync(image);
            _propertyRepoMock.Setup(r => r.Get(propertyId)).ReturnsAsync(property);

            var anotherListerId = Guid.NewGuid();

            Assert.ThrowsAsync<UnauthorizedException>(async () =>
            {
                await _service.DeleteImageAsync(imageId, anotherListerId, "Lister");
            });
        }

        [Test]
        public async Task DeleteImageAsync_AdminUser_DeletesSuccessfully()
        {
            var imageId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            var adminId = Guid.NewGuid();

            var image = new PropertyImage { Id = imageId, PropertyId = propertyId, ImageUrl = "/images/adminimg.jpg" };
            var property = new Property { Id = propertyId, ListerId = Guid.NewGuid() };

            var filePath = Path.Combine(_testWebRootPath, "images", "adminimg.jpg");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            await File.WriteAllTextAsync(filePath, "admin file");

            _imageRepoMock.Setup(r => r.Get(imageId)).ReturnsAsync(image);
            _propertyRepoMock.Setup(r => r.Get(propertyId)).ReturnsAsync(property);
            _imageRepoMock.Setup(r => r.Delete(imageId)).ReturnsAsync(image);


            await _service.DeleteImageAsync(imageId, adminId, "Admin");

            Assert.That(File.Exists(filePath), Is.False);
            _imageRepoMock.Verify(r => r.Delete(imageId), Times.Once);
        }


        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_testWebRootPath))
                Directory.Delete(_testWebRootPath, true);
        }
    }
}

