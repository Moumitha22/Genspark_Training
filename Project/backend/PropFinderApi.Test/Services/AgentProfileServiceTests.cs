using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Mappers;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Services;

namespace PropFinderApi.Tests.Services
{
    [TestFixture]
    public class AgentProfileServiceTests
    {
        private Mock<IAgentProfileRepository> _agentProfileRepoMock;
        private Mock<IRepository<Guid, User>> _userRepoMock;
        private AgentProfileService _service;

        [SetUp]
        public void Setup()
        {
            _agentProfileRepoMock = new Mock<IAgentProfileRepository>();
            _userRepoMock = new Mock<IRepository<Guid, User>>();
            _service = new AgentProfileService(_agentProfileRepoMock.Object, _userRepoMock.Object);
        }

        [Test]
        public async Task CreateAgentProfileAsync_Success()
        {
            var userId = Guid.NewGuid();
            var dto = new AgentProfileAddRequestDto
            {
                AgencyName = "Test Realty",
                BusinessPhoneNumber = "1234567890"
            };

            var user = new User { Id = userId };
            _userRepoMock.Setup(r => r.Get(userId)).ReturnsAsync(user);
            _agentProfileRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((AgentProfile?)null);

            var expectedProfile = new AgentProfile
            {
                UserId = userId,
                AgencyName = dto.AgencyName,
                BusinessPhoneNumber = dto.BusinessPhoneNumber
            };
            _agentProfileRepoMock.Setup(r => r.Add(It.IsAny<AgentProfile>())).ReturnsAsync(expectedProfile);

            var result = await _service.CreateAgentProfileAsync(dto, userId);

            Assert.IsNotNull(result);
            Assert.That(result.AgencyName, Is.EqualTo(dto.AgencyName));
            Assert.That(result.BusinessPhoneNumber, Is.EqualTo(dto.BusinessPhoneNumber));
        }

        [Test]
        public void CreateAgentProfileAsync_ProfileExists_ThrowsConflict()
        {
            var userId = Guid.NewGuid();
            var dto = new AgentProfileAddRequestDto
            {
                AgencyName = "Test Realty",
                BusinessPhoneNumber = "1234567890"
            };
            var user = new User { Id = userId };
            var existingProfile = new AgentProfile { UserId = userId };

            _userRepoMock.Setup(r => r.Get(userId)).ReturnsAsync(user);
            _agentProfileRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(existingProfile);

            Assert.ThrowsAsync<ConflictException>(() =>
                _service.CreateAgentProfileAsync(dto, userId));
        }

        [Test]
        public async Task GetAllAsync_ReturnsList()
        {
            var profiles = new List<AgentProfile>
            {
                new AgentProfile { AgencyName = "One" },
                new AgentProfile { AgencyName = "Two" }
            };

            _agentProfileRepoMock.Setup(r => r.GetAll()).ReturnsAsync(profiles);

            var result = await _service.GetAllAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsProfile()
        {
            var id = Guid.NewGuid();
            var profile = new AgentProfile { Id = id };

            _agentProfileRepoMock.Setup(r => r.Get(id)).ReturnsAsync(profile);

            var result = await _service.GetByIdAsync(id);

            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(id));
        }

        [Test]
        public void GetByIdAsync_ProfileNotFound_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();

            _agentProfileRepoMock.Setup(r => r.Get(id)).ThrowsAsync(new NotFoundException("Profile not found"));

            Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _service.GetByIdAsync(id);
            });
        }

        [Test]
        public async Task GetAgentProfileByAgentIdAsync_ReturnsProfile()
        {
            var agentId = Guid.NewGuid();
            var profile = new AgentProfile { UserId = agentId };

            _agentProfileRepoMock.Setup(r => r.GetByUserIdAsync(agentId)).ReturnsAsync(profile);

            var result = await _service.GetAgentProfileByAgentIdAsync(agentId);

            Assert.IsNotNull(result);
            Assert.That(result.UserId, Is.EqualTo(agentId));
        }

        [Test]
        public async Task GetAgentProfileByAgentIdAsync_ProfileNotFound_ReturnsNull()
        {
            var agentId = Guid.NewGuid();

            _agentProfileRepoMock.Setup(r => r.GetByUserIdAsync(agentId)).ReturnsAsync((AgentProfile?)null);

            var result = await _service.GetAgentProfileByAgentIdAsync(agentId);

            Assert.IsNull(result);
        }


        [Test]
        public async Task UpdateAgentProfileAsync_Success()
        {
            var profileId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var dto = new AgentProfileAddRequestDto
            {
                AgencyName = "Updated Co",
                BusinessPhoneNumber = "0000000000"
            };

            var existing = new AgentProfile
            {
                Id = profileId,
                UserId = requesterId,
                AgencyName = "Old Co",
                BusinessPhoneNumber = "1111111111"
            };

            _agentProfileRepoMock.Setup(r => r.Get(profileId)).ReturnsAsync(existing);
            _agentProfileRepoMock
                .Setup(r => r.Update(profileId, It.IsAny<AgentProfile>()))
                .ReturnsAsync((Guid id, AgentProfile updated) => updated);

            var result = await _service.UpdateAgentProfileAsync(profileId, dto, requesterId, "Agent");

            Assert.That(result.AgencyName, Is.EqualTo(dto.AgencyName));
            Assert.That(result.BusinessPhoneNumber, Is.EqualTo(dto.BusinessPhoneNumber));
        }

        [Test]
        public void UpdateAgentProfileAsync_Unauthorized_Throws()
        {
            var profileId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var dto = new AgentProfileAddRequestDto
            {
                AgencyName = "Updated Co",
                BusinessPhoneNumber = "9876543210"
            };

            var profile = new AgentProfile { Id = profileId, UserId = Guid.NewGuid() };

            _agentProfileRepoMock.Setup(r => r.Get(profileId)).ReturnsAsync(profile);

            Assert.ThrowsAsync<UnauthorizedException>(() =>
                _service.UpdateAgentProfileAsync(profileId, dto, requesterId, "Agent"));
        }
    }
}
