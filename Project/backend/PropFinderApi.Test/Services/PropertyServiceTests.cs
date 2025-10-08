using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Moq;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Mappers;
using PropFinderApi.Misc;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Services;

namespace PropFinderApi.Tests.Services
{
    [TestFixture]
    public class PropertyServiceTests
    {
        private Mock<IPropertyRepository> _propertyRepoMock;
        private Mock<IAgentProfileRepository> _agentProfileRepoMock;
        private Mock<IHubContext<NotificationHub>> _hubContextMock;
        private Mock<IHubClients> _hubClientsMock;
        private Mock<IClientProxy> _clientProxyMock;
        private PropertyMapper _mapper;


        private IPropertyService _service;

        [SetUp]
        public void Setup()
        {
            _propertyRepoMock = new Mock<IPropertyRepository>();
            _agentProfileRepoMock = new Mock<IAgentProfileRepository>();
            _hubContextMock = new Mock<IHubContext<NotificationHub>>();
            _hubClientsMock = new Mock<IHubClients>();
            _clientProxyMock = new Mock<IClientProxy>();
            _hubClientsMock.Setup(c => c.All).Returns(_clientProxyMock.Object);
            _clientProxyMock
                .Setup(c => c.SendCoreAsync(
                    It.IsAny<string>(),
                    It.IsAny<object[]>(),
                    default
                ))
                .Returns(Task.CompletedTask);
            _mapper = new PropertyMapper();
            _hubContextMock.Setup(c => c.Clients).Returns(_hubClientsMock.Object);

            _service = new PropertyService(
                _propertyRepoMock.Object,
                _agentProfileRepoMock.Object,
                _hubContextMock.Object
            );
        }

        [Test]
        public async Task CreatePropertyAsync_Success()
        {
            // Arrange
            var agentId = Guid.NewGuid();
            var dto = new PropertyAddRequestDto
            {
                Title = "Nice Flat",
                Location = "Bangalore",
                Price = 5000000
            };

            var agentProfile = new AgentProfile
            {
                UserId = agentId,
                BusinessPhoneNumber = "9876543210"
            };

            var property = new Property
            {
                Title = dto.Title,
                Location = dto.Location,
                Price = dto.Price,
                AgentId = agentId
            };

            _agentProfileRepoMock
                .Setup(r => r.GetByUserIdAsync(agentId))
                .ReturnsAsync(agentProfile);

            _propertyRepoMock
                .Setup(r => r.Add(It.IsAny<Property>()))
                .ReturnsAsync(property);

            var result = await _service.CreatePropertyAsync(dto, agentId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Title, Is.EqualTo(dto.Title));
            Assert.That(result.Location, Is.EqualTo(dto.Location));
            Assert.That(result.Price, Is.EqualTo(dto.Price));
            Assert.That(result.AgentId, Is.EqualTo(agentId));
        }


        [Test]
        public void CreatePropertyAsync_MissingProfile_ThrowsNotFound()
        {
            var agentId = Guid.NewGuid();

            var dto = new PropertyAddRequestDto
            {
                Title = "Nice Flat",
                Location = "Bangalore",
                Price = 5000000
            };

            _agentProfileRepoMock.Setup(r => r.GetByUserIdAsync(agentId)).ReturnsAsync((AgentProfile)null);

            Assert.ThrowsAsync<NotFoundException>(() => _service.CreatePropertyAsync(dto, agentId));
        }

        [Test]
        public void CreatePropertyAsync_MissingPhone_ThrowsBadRequest()
        {
            var agentId = Guid.NewGuid();
            var dto = new PropertyAddRequestDto
            {
                Title = "Nice Flat",
                Location = "Bangalore",
                Price = 5000000
            };
            var profile = new AgentProfile { UserId = agentId, BusinessPhoneNumber = null };

            _agentProfileRepoMock.Setup(r => r.GetByUserIdAsync(agentId)).ReturnsAsync(profile);

            Assert.ThrowsAsync<BadRequestException>(() => _service.CreatePropertyAsync(dto, agentId));
        }

        [Test]
        public async Task GetAllPropertiesAsync_ReturnsList()
        {
            var sampleProperties = new List<Property>
            {
                new Property
                {
                    Id = Guid.NewGuid(),
                    Title = "Lake View Apartment",
                    Location = "Hyderabad",
                    Price = 8000000,
                    AgentId = Guid.NewGuid()
                },
                new Property
                {
                    Id = Guid.NewGuid(),
                    Title = "Green Villa",
                    Location = "Bangalore",
                    Price = 15000000,
                    AgentId = Guid.NewGuid()
                }
            };

            _propertyRepoMock.Setup(r => r.GetAll()).ReturnsAsync(sampleProperties);

            // Act
            var result = (await _service.GetAllPropertiesAsync()).ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Title, Is.EqualTo("Lake View Apartment"));
            Assert.That(result[0].Location, Is.EqualTo("Hyderabad"));
            Assert.That(result[1].Title, Is.EqualTo("Green Villa"));
            Assert.That(result[1].Location, Is.EqualTo("Bangalore"));
        }

        [Test]
        public async Task GetPropertyByIdAsync_Success()
        {
            var id = Guid.NewGuid();
            var sample = new Property { Id = id, Title = "Sunrise Villa", Location = "Goa" };

            _propertyRepoMock.Setup(r => r.Get(id)).ReturnsAsync(sample);

            var result = await _service.GetPropertyByIdAsync(id);

            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.Title, Is.EqualTo("Sunrise Villa"));
        }

        [Test]
        public void GetPropertyByIdAsync_NotFound_Throws()
        {
            var id = Guid.NewGuid();
            _propertyRepoMock.Setup(r => r.Get(id)).ThrowsAsync(new NotFoundException("not found"));

            Assert.ThrowsAsync<NotFoundException>(() => _service.GetPropertyByIdAsync(id));
        }

        [Test]
        public async Task GetPropertiesByAgentIdAsync_ReturnsList()
        {
            var agentId = Guid.NewGuid();
            var list = new List<Property>
            {
                new Property { Id = Guid.NewGuid(), Title = "Ocean Breeze", AgentId = agentId },
                new Property { Id = Guid.NewGuid(), Title = "City Heights", AgentId = agentId }
            };

            _propertyRepoMock.Setup(r => r.GetByAgentIdAsync(agentId, false)).ReturnsAsync(list);

            var result = await _service.GetPropertiesByAgentIdAsync(agentId);

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        
        [Test]
        public async Task UpdatePropertyAsync_UpdatesFieldsCorrectly()
        {
            var id = Guid.NewGuid();
            var dto = new PropertyUpdateRequestDto
            {
                Title = "Updated Villa",
                Location = "Mumbai",
                Price = 12000000
            };

            var property = new Property
            {
                Id = id,
                Title = "Old Villa",
                Location = "Delhi",
                Price = 10000000,
                AgentId = id
            };

            _propertyRepoMock.Setup(r => r.Get(id)).ReturnsAsync(property);
            _propertyRepoMock.Setup(r => r.Update(id, It.IsAny<Property>()))
                .ReturnsAsync((Guid id, Property updatedProp) => updatedProp);

            var result = await _service.UpdatePropertyAsync(id, dto, id, "Agent");

            Assert.IsNotNull(result);
            Assert.That(result.Title, Is.EqualTo("Updated Villa"));
            Assert.That(result.Location, Is.EqualTo("Mumbai"));
            Assert.That(result.Price, Is.EqualTo(12000000));
        }


        [Test]
        public void UpdatePropertyAsync_Unauthorized_Throws()
        {
            var id = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
             var propertyDto = new PropertyUpdateRequestDto
            {
                Title = "Sample title"
            };
            var property = new Property { Id = id, AgentId = Guid.NewGuid() }; 
            _propertyRepoMock.Setup(r => r.Get(id)).ReturnsAsync(property);

            Assert.ThrowsAsync<UnauthorizedException>(() =>
                _service.UpdatePropertyAsync(id, new PropertyUpdateRequestDto(), requesterId, "Agent"));
        }

        [Test]
        public void UpdatePropertyAsync_NotFound_Throws()
        {
            var id = Guid.NewGuid();

            var propertyDto = new PropertyUpdateRequestDto
            {
                Title = "Sample title"
            };

            _propertyRepoMock.Setup(r => r.Get(id)).ThrowsAsync(new NotFoundException("not found"));

            Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdatePropertyAsync(id, propertyDto, id, "Agent"));
        }

        [Test]
        public async Task UpdatePropertyStatusAsync_UpdatesStatus()
        {
            var id = Guid.NewGuid();
            var agentId = Guid.NewGuid();
            var requesterId = agentId;
            var property = new Property
            {
                Id = id,
                AgentId = agentId,
                Status = "Available"
            };

            _propertyRepoMock.Setup(r => r.Get(id)).ReturnsAsync(property);

            await _service.UpdatePropertyStatusAsync(id, "Sold", requesterId, "Agent");

            _propertyRepoMock.Verify(r => r.UpdateStatusAsync(id, "Sold"), Times.Once);
        }


        [Test]
        public void UpdatePropertyStatusAsync_Unauthorized_Throws()
        {
            var id = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var property = new Property { Id = id, AgentId = Guid.NewGuid() };
            _propertyRepoMock.Setup(r => r.Get(id)).ReturnsAsync(property);

            Assert.ThrowsAsync<UnauthorizedException>(() =>
                _service.UpdatePropertyStatusAsync(id, "Sold", requesterId, "Agent"));
        }

        [Test]
        public void UpdatePropertyStatusAsync_NotFound_Throws()
        {
            var id = Guid.NewGuid();
            _propertyRepoMock.Setup(r => r.Get(id)).ThrowsAsync(new NotFoundException("not found"));

            Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdatePropertyStatusAsync(id, "Sold", id, "Agent"));
        }

        [Test]
        public async Task DeletePropertyAsync_SetsIsDeletedTrue()
        {
            var id = Guid.NewGuid();
            var agentId = Guid.NewGuid();
            var requesterId = agentId;
            var property = new Property { Id = id, AgentId = agentId, IsDeleted = false };

            _propertyRepoMock.Setup(r => r.Get(id)).ReturnsAsync(property);
            _propertyRepoMock
                .Setup(r => r.Update(id, It.Is<Property>(p => p.IsDeleted)))
                .ReturnsAsync((Guid id, Property p) => p);

            await _service.DeletePropertyAsync(id, requesterId, "Agent");

            _propertyRepoMock.Verify(r => r.Update(id, It.Is<Property>(p => p.IsDeleted)), Times.Once);
        }


        [Test]
        public void DeletePropertyAsync_Unauthorized_Throws()
        {
            var id = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var property = new Property { Id = id, AgentId = Guid.NewGuid() };
            _propertyRepoMock.Setup(r => r.Get(id)).ReturnsAsync(property);

            Assert.ThrowsAsync<UnauthorizedException>(() =>
                _service.DeletePropertyAsync(id, requesterId, "Agent"));
        }

        [Test]
        public void DeletePropertyAsync_NotFound_Throws()
        {
            var id = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            _propertyRepoMock.Setup(r => r.Get(id)).ThrowsAsync(new NotFoundException("not found"));

            Assert.ThrowsAsync<NotFoundException>(() =>
                _service.DeletePropertyAsync(id, requesterId, "Agent"));
        }

        [Test]
        public async Task SearchPropertiesAsync_ReturnsFilteredProperties()
        {
            var searchModel = new PropertySearchModel { Location = "Delhi" };
            var sortModel = new SortModel { SortBy = "price", Ascending= false };

            var properties = new List<Property>
            {
                new Property { Title = "Urban Flat", Location = "Delhi" },
                new Property { Title = "Riverside Home", Location = "Delhi" }
            };

            _propertyRepoMock.Setup(r => r.SearchPropertiesAsync(searchModel, sortModel))
                .ReturnsAsync(properties);

            var result = await _service.SearchPropertiesAsync(searchModel, sortModel);

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.IsTrue(result.All(p => p.Location == "Delhi"));
        }


    }
}
