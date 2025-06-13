using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PropFinderApi.Controllers;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Tests.Controllers
{
    public class PropertyImageControllerTests
    {
        private Mock<IPropertyImageService> _propertyImageServiceMock;
        private Mock<IApiResponseMapper> _responseMapperMock;
        private PropertyImageController _controller;

        [SetUp]
        public void SetUp()
        {
            _propertyImageServiceMock = new Mock<IPropertyImageService>();
            _responseMapperMock = new Mock<IApiResponseMapper>();
            _controller = new PropertyImageController(_propertyImageServiceMock.Object, _responseMapperMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Agent")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task Upload_ValidImage_ReturnsOk()
        {
            var imageUploadDto = new PropertyImageUploadDto
            {
                PropertyId = Guid.NewGuid(),
                File = new FormFile(new MemoryStream(new byte[10]), 0, 10, "file", "test.jpg")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
                }
            };

            var uploadedImage = new PropertyImage { Id = Guid.NewGuid(), ImageUrl = "images/test.jpg" };
            var response = new ApiResponse<PropertyImage>();

            _propertyImageServiceMock.Setup(s => s.UploadImageAsync(imageUploadDto, It.IsAny<Guid>())).ReturnsAsync(uploadedImage);
            _responseMapperMock.Setup(m => m.MapToOkResponse("Uploaded image successfully", uploadedImage)).Returns(response);

            var result = await _controller.Upload(imageUploadDto);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void Upload_NullFile_ThrowsBadRequest()
        {
            var dto = new PropertyImageUploadDto
            {
                PropertyId = Guid.NewGuid(),
                File = null
            };

            var ex = Assert.ThrowsAsync<BadRequestException>(() => _controller.Upload(dto));
            Assert.That(ex.Message, Is.EqualTo("Invalid file."));
        }

        [Test]
        public async Task Get_ImageByPropertyId_ReturnsOk()
        {
            var propertyId = Guid.NewGuid();
            var images = new List<PropertyImage>();
            var response = new ApiResponse<List<PropertyImage>>();

            _propertyImageServiceMock.Setup(s => s.GetImagesByPropertyIdAsync(propertyId)).ReturnsAsync(images);
            _responseMapperMock.Setup(m => m.MapToOkResponse("Images fetched successfully", images)).Returns(response);

            var result = await _controller.Get(propertyId);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void Get_InvalidPropertyId_ThrowsBadRequest()
        {
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _controller.Get(Guid.Empty));
            Assert.That(ex.Message, Is.EqualTo("Invalid property ID."));
        }

        [Test]
        public async Task GetImageById_ValidId_ReturnsFileResult()
        {
            var id = Guid.NewGuid();
            var bytes = new byte[] { 0x01, 0x02 };
            var contentType = "image/png";

            _propertyImageServiceMock.Setup(s => s.GetImageContentByIdAsync(id)).ReturnsAsync((bytes, contentType));

            var result = await _controller.GetImageById(id);

            Assert.That(result, Is.InstanceOf<FileContentResult>());
            var fileResult = result as FileContentResult;
            Assert.That(fileResult?.ContentType, Is.EqualTo("image/png"));
            Assert.That(fileResult?.FileContents.Length, Is.EqualTo(2));
        }
    }
}
