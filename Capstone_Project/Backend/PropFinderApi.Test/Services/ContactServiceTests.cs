using Moq;
using NUnit.Framework;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Services;

namespace PropFinderApi.Tests.Services
{
    public class ContactServiceTests
    {
        private Mock<IContactLogRepository> _contactLogRepoMock;
        private Mock<IUserRepository> _userRepoMock;
        private Mock<IAgentProfileRepository> _agentProfileRepoMock;
        private Mock<IRepository<Guid, Property>> _propertyRepoMock;

        private ContactService _service;

        [SetUp]
        public void Setup()
        {
            _contactLogRepoMock = new Mock<IContactLogRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _agentProfileRepoMock = new Mock<IAgentProfileRepository>();
            _propertyRepoMock = new Mock<IRepository<Guid, Property>>();

            _service = new ContactService(
                _contactLogRepoMock.Object,
                _userRepoMock.Object,
                _agentProfileRepoMock.Object,
                _propertyRepoMock.Object);
        }

        [Test]
        public async Task ContactAgentAsync_Success_ReturnsResponse()
        {
            var buyerId = Guid.NewGuid();
            var agentId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();

            var buyer = new User
            {
                Id = buyerId,
                Email = "buyer@example.com",
                PhoneNumber = ""
            };

            var agent = new User
            {
                Id = agentId,
                Name = "Agent John",
                Email = "john@agent.com",
                AgentProfile = new AgentProfile { BusinessPhoneNumber = "9876543210" }
            };

            var property = new Property
            {
                Id = propertyId,
                AgentId = agentId
            };

            var dto = new ContactAgentRequestDto
            {
                PropertyId = propertyId,
                BuyerPhoneNumber = "9999999999"
            };

            _userRepoMock.Setup(r => r.Get(buyerId)).ReturnsAsync(buyer);
            _userRepoMock.Setup(r => r.Update(buyerId, It.IsAny<User>())).ReturnsAsync(buyer);
            _propertyRepoMock.Setup(r => r.Get(propertyId)).ReturnsAsync(property);
            _userRepoMock.Setup(r => r.GetWithProfileAsync(agentId)).ReturnsAsync(agent);
            _contactLogRepoMock.Setup(r => r.Add(It.IsAny<ContactLog>())).ReturnsAsync(new ContactLog());

            var result = await _service.ContactAgentAsync(dto, buyerId);

            Assert.That(result.AgentEmail, Is.EqualTo(agent.Email));
            Assert.That(result.AgentName, Is.EqualTo(agent.Name));
            Assert.That(result.AgentPhoneNumber, Is.EqualTo(agent.AgentProfile.BusinessPhoneNumber));
        }

        [Test]
        public void ContactAgentAsync_ThrowsNotFound_WhenPropertyMissing()
        {
            var buyerId = Guid.NewGuid();
            var agentId = Guid.NewGuid();

            var buyer = new User
            {
                Id = buyerId,
                Email = "buyer@example.com",
                PhoneNumber = ""
            };

            var agent = new User
            {
                Id = agentId,
                Name = "Agent John",
                Email = "john@agent.com",
                AgentProfile = new AgentProfile { BusinessPhoneNumber = "9876543210" }
            };

            var dto = new ContactAgentRequestDto
            {
                PropertyId = Guid.NewGuid(),
                BuyerPhoneNumber = "999"
            };

            _userRepoMock.Setup(r => r.Get(buyerId)).ReturnsAsync(new User());
            _propertyRepoMock.Setup(r => r.Get(dto.PropertyId)).ThrowsAsync(new NotFoundException("Not found"));

            Assert.ThrowsAsync<NotFoundException>(() => _service.ContactAgentAsync(dto, buyerId));
        }

        [Test]
        public async Task GetContactLogsForAgentAsync_AsAdmin_ReturnsMultipleLogs()
        {
            var agentId = Guid.NewGuid();

            var logs = new List<ContactLog>
            {
                new ContactLog
                {
                    Id = Guid.NewGuid(),
                    AgentId = agentId,
                    BuyerId = Guid.NewGuid(),
                    PropertyId = Guid.NewGuid(),
                },
                new ContactLog
                {
                    Id = Guid.NewGuid(),
                    AgentId = agentId,
                    BuyerId = Guid.NewGuid(),
                    PropertyId = Guid.NewGuid(),
                }
            };

            _contactLogRepoMock.Setup(r => r.GetByAgentIdAsync(agentId)).ReturnsAsync(logs);

            var result = (await _service.GetContactLogsForAgentAsync(agentId, Guid.NewGuid(), "Admin")).ToList();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(r => r.AgentId == agentId));
        }

        [Test]
        public async Task GetContactLogsForAgentAsync_AsSameAgent_ReturnsMultipleLogs()
        {
            var agentId = Guid.NewGuid();
            var requesterId = agentId;

            var logs = new List<ContactLog>
            {
                new ContactLog
                {
                    Id = Guid.NewGuid(),
                    AgentId = agentId,
                    BuyerId = Guid.NewGuid(),
                    PropertyId = Guid.NewGuid(),
                },
                new ContactLog
                {
                    Id = Guid.NewGuid(),
                    AgentId = agentId,
                    BuyerId = Guid.NewGuid(),
                    PropertyId = Guid.NewGuid(),
                }
            };

            _contactLogRepoMock.Setup(r => r.GetByAgentIdAsync(agentId)).ReturnsAsync(logs);

            var result = (await _service.GetContactLogsForAgentAsync(agentId, requesterId, "Agent")).ToList();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(r => r.AgentId == agentId));
        }


        [Test]
        public void GetContactLogsForAgentAsync_AsAgentOtherThanSelf_ThrowsUnauthorized()
        {
            var agentId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();

            var logs = new List<ContactLog>
            {
                new ContactLog
                {
                    Id = Guid.NewGuid(),
                    AgentId = agentId,
                    BuyerId = Guid.NewGuid(),
                    PropertyId = Guid.NewGuid(),
                },
                new ContactLog
                {
                    Id = Guid.NewGuid(),
                    AgentId = agentId,
                    BuyerId = Guid.NewGuid(),
                    PropertyId = Guid.NewGuid(),
                }
            };

            _contactLogRepoMock.Setup(r => r.GetByAgentIdAsync(agentId)).ReturnsAsync(logs);


            Assert.ThrowsAsync<UnauthorizedException>(() =>
                _service.GetContactLogsForAgentAsync(agentId, requesterId, "Agent"));
        }

        [Test]
        public async Task GetContactLogsForBuyerAsync_AsAdmin_ReturnsMultipleLogs()
        {
            var buyerId = Guid.NewGuid();

            var logs = new List<ContactLog>
            {
                new ContactLog
                {
                    Id = Guid.NewGuid(),
                    BuyerId = buyerId,
                    AgentId = Guid.NewGuid(),
                    PropertyId = Guid.NewGuid()
                },
                new ContactLog
                {
                    Id = Guid.NewGuid(),
                    BuyerId = buyerId,
                    AgentId = Guid.NewGuid(),
                    PropertyId = Guid.NewGuid()
                },
                new ContactLog
                {
                    Id = Guid.NewGuid(),
                    BuyerId = buyerId,
                    AgentId = Guid.NewGuid(),
                    PropertyId = Guid.NewGuid()
                }
            };

            _contactLogRepoMock.Setup(r => r.GetByBuyerIdAsync(buyerId)).ReturnsAsync(logs);

            var result = (await _service.GetContactLogsForBuyerAsync(buyerId, Guid.NewGuid(), "Admin")).ToList();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result.All(r => r.BuyerId == buyerId));
        }

        [Test]
        public async Task GetContactLogsForBuyerAsync_AsSameBuyer_ReturnsLogs()
        {
            var buyerId = Guid.NewGuid();

            var logs = new List<ContactLog>
            {
                new ContactLog { Id = Guid.NewGuid(), BuyerId = buyerId }
            };

            _contactLogRepoMock.Setup(r => r.GetByBuyerIdAsync(buyerId)).ReturnsAsync(logs);

            var result = await _service.GetContactLogsForBuyerAsync(buyerId, buyerId, "Agent");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().BuyerId, Is.EqualTo(buyerId));
        }
        
        [Test]
        public void GetContactLogsForBuyerAsync_AsBuyerOtherThanSelf_ThrowsUnauthorized()
        {
            var buyerId = Guid.NewGuid();
            var otherId = Guid.NewGuid();

            var logs = new List<ContactLog>
            {
                new ContactLog { Id = Guid.NewGuid(), BuyerId = buyerId }
            };

            _contactLogRepoMock.Setup(r => r.GetByBuyerIdAsync(buyerId)).ReturnsAsync(logs);


            Assert.ThrowsAsync<UnauthorizedException>(() =>
                _service.GetContactLogsForBuyerAsync(buyerId, otherId, "Buyer"));
        }

    }
}
