using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PropFinderApi.Controllers;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Services;

namespace PropFinderApi.Tests.Controllers
{
    public class PropertyControllerTests
    {
        private Mock<IPropertyService> _propertyServiceMock;
        private Mock<IPaginationService> _paginationServiceMock;
        private Mock<IApiResponseMapper> _responseMapperMock;
        private PropertyController _controller;

        [SetUp]
        public void Setup()
        {
            _propertyServiceMock = new Mock<IPropertyService>();
            _paginationServiceMock = new Mock<IPaginationService>();
            _responseMapperMock = new Mock<IApiResponseMapper>();

            _controller = new PropertyController(
                _propertyServiceMock.Object,
                _paginationServiceMock.Object,
                _responseMapperMock.Object
            );
        }

        private void SetUserContext(Guid userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var user = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task CreateProperty_ReturnsOkResponse_WhenAgent()
        {
            var userId = Guid.NewGuid();
            SetUserContext(userId, "Agent");

            var dto = new PropertyAddRequestDto
            {
                Title = "3BHK Flat in Mumbai",
                Description = "Spacious flat with sea view",
                Price = 12000000,
                Location = "Marine Drive, Mumbai",
            };

            var created = new Property
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price
            };

            var response = new ApiResponse<Property>
            {
                Success = true,
                Message = "Property created successfully",
                Data = created
            };

            _propertyServiceMock.Setup(s => s.CreatePropertyAsync(dto, userId))
                                .ReturnsAsync(created);

            _responseMapperMock.Setup(m => m.MapToOkResponse(It.IsAny<string>(), created))
                            .Returns(response);

            var result = await _controller.Create(dto);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(response));
        }

        [Test]
        public async Task GetAll_ReturnsOk_WithPagination()
        {
            var list = new List<PropertyResponseDto>
            {
                new PropertyResponseDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Luxury Villa in Goa",
                    Description = "A beautiful sea-facing villa",
                    Price = 15000000,
                    PropertyType = "Villa",
                    Status = "Available"
                },
                new PropertyResponseDto
                {
                    Id = Guid.NewGuid(),
                    Title = "2BHK Apartment in Bangalore",
                    Description = "Close to IT parks and metro",
                    Price = 8500000,
                    PropertyType = "Apartment",
                    Status = "Sold"
                }
            };

            // Simulate pagination returning just 1st property for page 1, page size 1
            var paginated = new List<PropertyResponseDto> { list[0] };

            var pagination = new PaginationInfoDto
            {
                CurrentPage = 1,
                PageSize = 1,
                TotalItems = 2,
                TotalPages = 2
            };

            var response = new ApiResponse<List<PropertyResponseDto>>
            {
                Message = "All properties fetched successfully",
                Data = paginated,
                Pagination = pagination
            };

            // Setup mocks
            _propertyServiceMock.Setup(s => s.GetAllPropertiesAsync()).ReturnsAsync(list);
            _paginationServiceMock.Setup(p => p.ApplyPagination(list, 1, 1)).Returns((paginated, pagination));
            _responseMapperMock.Setup(m => m.MapToOkResponse("All properties fetched successfully", paginated, pagination)).Returns(response);

            // Act
            var result = await _controller.GetAll(1, 1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var responseData = okResult.Value as ApiResponse<List<PropertyResponseDto>>;
            Assert.IsNotNull(responseData);
            Assert.That(responseData.Data.Count, Is.EqualTo(1));
            Assert.That(responseData.Data[0].Title, Is.EqualTo("Luxury Villa in Goa"));
        }

        [Test]
        public async Task GetById_ReturnsOk()
        {
            var property = new PropertyResponseDto();
            var response = new ApiResponse<PropertyResponseDto>();
            var id = Guid.NewGuid();

            _propertyServiceMock.Setup(s => s.GetPropertyByIdAsync(id)).ReturnsAsync(property);
            _responseMapperMock.Setup(m => m.MapToOkResponse(It.IsAny<string>(), property)).Returns(response);

            var result = await _controller.GetById(id);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void GetById_ThrowsBadRequest_WhenEmptyGuid()
        {
            Assert.ThrowsAsync<BadRequestException>(() => _controller.GetById(Guid.Empty));
        }

        [Test]
        public async Task UpdateProperty_ReturnsOk_WhenAgentOwnsProperty()
        {
            var propertyId = Guid.NewGuid();
            var agentId = Guid.NewGuid();
            SetUserContext(agentId, "Agent");

            var updateDto = new PropertyUpdateRequestDto
            {
                Title = "Updated Luxury Apartment",
                Description = "Updated modern apartment with amenities",
                Price = 175000
            };

            var updatedProperty = new PropertyResponseDto
            {
                Id = propertyId,
                Title = updateDto.Title,
                Description = updateDto.Description,
                Price = updateDto.Price?? 0,
                Location = "Delhi"
            };

            var expectedResponse = new ApiResponse<PropertyResponseDto>
            {
                Data = updatedProperty,
                Message = "Property updated successfully"
            };

            _propertyServiceMock
                .Setup(s => s.UpdatePropertyAsync(propertyId, updateDto, agentId, "Agent"))
                .ReturnsAsync(updatedProperty);

            _responseMapperMock
                .Setup(m => m.MapToOkResponse("Property updated successfully", updatedProperty))
                .Returns(expectedResponse);

            var result = await _controller.UpdateProperty(propertyId, updateDto);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var apiResult = okResult.Value as ApiResponse<PropertyResponseDto>;
            Assert.IsNotNull(apiResult);
            Assert.That(apiResult.Data.Id, Is.EqualTo(propertyId));
            Assert.That(apiResult.Data.Title, Is.EqualTo(updateDto.Title));
            Assert.That(apiResult.Message, Is.EqualTo("Property updated successfully"));

            _propertyServiceMock.Verify(s => s.UpdatePropertyAsync(propertyId, updateDto, agentId, "Agent"), Times.Once);
        }


        [Test]
        public async Task DeleteProperty_CallsService_WhenValid()
        {
            var id = Guid.NewGuid();
            var userId = Guid.NewGuid();
            SetUserContext(userId, "Agent");

            var response = new ApiResponse<object>
            {
                Message = "Property deleted successfully"
            };

            _responseMapperMock.Setup(m => m.MapToOkResponse("Property deleted successfully")).Returns(response);

            var result = await _controller.Delete(id);

            _propertyServiceMock.Verify(s => s.DeletePropertyAsync(id, userId, "Agent"), Times.Once);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var resultValue = okResult.Value as ApiResponse<object>;
            Assert.IsNotNull(resultValue);
            Assert.That(resultValue.Message, Is.EqualTo("Property deleted successfully"));
        }


        [Test]
        public async Task Search_ReturnsOk()
        {
            var searchModel = new PropertySearchModel
            {
                Location = "Delhi"
            };

            var sortModel = new SortModel
            {
                SortBy = "price",
                Ascending = false
            };

            var sampleProperties = new List<PropertyResponseDto>
            {
                new PropertyResponseDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Urban Flat",
                    Description = "A modern flat in the heart of Delhi",
                    Location = "Delhi",
                    Price = 150000,
                    Status = "Available"
                },
                new PropertyResponseDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Riverside Home",
                    Description = "Beautiful riverside home in Delhi",
                    Location = "Delhi",
                    Price = 120000,
                    Status = "Available"
                }
            };

            var pagedProperties = sampleProperties.Take(2).ToList();

            var pagination = new PaginationInfoDto
            {
                CurrentPage = 1,
                PageSize = 10,
                TotalItems = sampleProperties.Count,
                TotalPages = 1
            };

            var expectedResponse = new ApiResponse<List<PropertyResponseDto>>
            {
                Data = pagedProperties,
                Pagination = pagination,
                Message = "All properties fetched successfully"
            };

            _propertyServiceMock
                .Setup(s => s.SearchPropertiesAsync(searchModel, sortModel))
                .ReturnsAsync(sampleProperties);

            _paginationServiceMock
                .Setup(p => p.ApplyPagination(sampleProperties, 1, 10))
                .Returns((pagedProperties, pagination));

            _responseMapperMock
                .Setup(m => m.MapToOkResponse("All properties fetched successfully", pagedProperties, pagination))
                .Returns(expectedResponse);

            var result = await _controller.Search(searchModel, sortModel, 1, 10);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var apiResult = okResult.Value as ApiResponse<List<PropertyResponseDto>>;
            Assert.IsNotNull(apiResult);
            Assert.That(apiResult.Data.Count, Is.EqualTo(2));
            Assert.That(apiResult.Data[0].Location, Is.EqualTo("Delhi"));
            Assert.That(apiResult.Pagination.TotalItems, Is.EqualTo(2));
        }
    }
}
