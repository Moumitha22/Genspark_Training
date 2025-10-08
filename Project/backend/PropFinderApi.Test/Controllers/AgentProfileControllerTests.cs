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

namespace PropFinderApi.Tests.Controllers
{
    [TestFixture]
    public class AgentProfileControllerTests
    {
        private Mock<IAgentProfileService> _agentServiceMock;
        private Mock<IPaginationService> _paginationServiceMock;
        private Mock<IApiResponseMapper> _responseMapperMock;
        private AgentProfileController _controller;

        [SetUp]
        public void Setup()
        {
            _agentServiceMock = new Mock<IAgentProfileService>();
            _paginationServiceMock = new Mock<IPaginationService>();
            _responseMapperMock = new Mock<IApiResponseMapper>();
            _controller = new AgentProfileController(
                _agentServiceMock.Object,
                _paginationServiceMock.Object,
                _responseMapperMock.Object);

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
        public async Task Create_ReturnsOkResult_WithCreatedProfile()
        {
            var dto = new AgentProfileAddRequestDto { LicenseNumber = "LIC123", BusinessPhoneNumber = "9876543210" };
            var created = new AgentProfile { Id = Guid.NewGuid(), LicenseNumber = "LIC123", BusinessPhoneNumber = "9876543210" };
            var response = new ApiResponse<AgentProfile> { Data = created };

            _agentServiceMock.Setup(x => x.CreateAgentProfileAsync(dto, It.IsAny<Guid>())).ReturnsAsync(created);
            _responseMapperMock.Setup(x => x.MapToOkResponse("Agent profile created successfully", created)).Returns(response);

            var result = await _controller.Create(dto);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(((OkObjectResult)result).Value, Is.EqualTo(response));
        }

        [Test]
        public async Task GetAll_ReturnsProfiles()
        {
            var profiles = new List<AgentProfile>
            {
                new AgentProfile { Id = Guid.NewGuid(), LicenseNumber = "LIC1" },
                new AgentProfile { Id = Guid.NewGuid(), LicenseNumber = "LIC2" }
            };
            var paginated = new List<AgentProfile> { profiles[0] };
            var meta = new PaginationInfoDto();
            var response = new ApiResponse<List<AgentProfile>>();

            _agentServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(profiles);
            _paginationServiceMock.Setup(p => p.ApplyPagination(profiles, 1, 10)).Returns((paginated, meta));
            _responseMapperMock.Setup(m => m.MapToOkResponse("All agent profiles fetched successfully", paginated, meta)).Returns(response);

            var result = await _controller.GetAll(1, 10);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetById_ReturnsOk_WhenIdIsValid()
        {
            var id = Guid.NewGuid();
            var dto = new AgentProfile { Id = id, LicenseNumber = "LIC123" };
            var response = new ApiResponse<AgentProfile>();

            _agentServiceMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(dto);
            _responseMapperMock.Setup(x => x.MapToOkResponse("Agent Profile fetched by ID", dto)).Returns(response);

            var result = await _controller.GetById(id);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetByAgentId_ReturnsOk_WhenFound()
        {
            var agentId = Guid.NewGuid();
            var dto = new AgentProfile { Id = Guid.NewGuid(), LicenseNumber = "LIC456" };
            var response = new ApiResponse<AgentProfile>();

            _agentServiceMock.Setup(x => x.GetAgentProfileByAgentIdAsync(agentId)).ReturnsAsync(dto);
            _responseMapperMock.Setup(x => x.MapToOkResponse("Agent profile fetched successfully", dto)).Returns(response);

            var result = await _controller.GetByAgentId(agentId);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void GetByAgentId_ThrowsNotFound_WhenNullReturned()
        {
            var agentId = Guid.NewGuid();
            _agentServiceMock.Setup(x => x.GetAgentProfileByAgentIdAsync(agentId)).ReturnsAsync((AgentProfile?)null);

            Assert.ThrowsAsync<NotFoundException>(() => _controller.GetByAgentId(agentId));
        }

        [Test]
        public async Task Update_ReturnsOk_WhenUpdated()
        {
            var profileId = Guid.NewGuid();
            var dto = new AgentProfileAddRequestDto { LicenseNumber = "LIC789", BusinessPhoneNumber = "1234567890" };
            var updated = new AgentProfile { Id = profileId, LicenseNumber = "LIC789" };
            var response = new ApiResponse<AgentProfile>();

            _agentServiceMock.Setup(x => x.UpdateAgentProfileAsync(profileId, dto, It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(updated);
            _responseMapperMock.Setup(x => x.MapToOkResponse("Agent profile updated successfully", updated)).Returns(response);

            var result = await _controller.Update(profileId, dto);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
    }
}