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
        public async Task UploadImageAsync_ValidAgent_UploadsSuccessfully()
        {
            var agentId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            var property = new Property { Id = propertyId, AgentId = agentId };

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

            var dto = new PropertyImageUploadDto { PropertyId = propertyId, File = fileMock.Object };

            var result = await _service.UploadImageAsync(dto, agentId);

            Assert.IsNotNull(result);
            Assert.That(result.ImageUrl, Does.StartWith("/images/"));
            Assert.That(File.Exists(Path.Combine(_testWebRootPath, result.ImageUrl.TrimStart('/'))));
        }

        [Test]
        public void UploadImageAsync_InvalidAgent_ThrowsUnauthorized()
        {
            var requesterId = Guid.NewGuid();
            var property = new Property { Id = Guid.NewGuid(), AgentId = Guid.NewGuid() }; // different agent

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.jpg");

            _propertyRepoMock.Setup(r => r.Get(property.Id)).ReturnsAsync(property);

            var dto = new PropertyImageUploadDto { PropertyId = property.Id, File = fileMock.Object };

            Assert.ThrowsAsync<UnauthorizedException>(async () =>
            {
                await _service.UploadImageAsync(dto, requesterId);
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
        public async Task GetImageContentByIdAsync_ValidId_ReturnsContent()
        {
            var fileName = "image.png";
            var imageId = Guid.NewGuid();
            var filePath = Path.Combine(_testWebRootPath, "images", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            await File.WriteAllTextAsync(filePath, "fake-image-data");

            var image = new PropertyImage { Id = imageId, ImageUrl = "/images/" + fileName };

            _imageRepoMock.Setup(r => r.Get(imageId)).ReturnsAsync(image);

            var (content, contentType) = await _service.GetImageContentByIdAsync(imageId);

            Assert.That(content.Length, Is.GreaterThan(0));
            Assert.That(contentType, Is.EqualTo("image/png"));
        }

        [Test]
        public void GetImageContentByIdAsync_FileNotFound_ThrowsException()
        {
            var imageId = Guid.NewGuid();
            var image = new PropertyImage { Id = imageId, ImageUrl = "/images/missing.jpg" };

            _imageRepoMock.Setup(r => r.Get(imageId)).ReturnsAsync(image);

            Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _service.GetImageContentByIdAsync(imageId);
            });
        }


        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_testWebRootPath))
                Directory.Delete(_testWebRootPath, true);
        }
    }
}

