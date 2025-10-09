using Microsoft.AspNetCore.SignalR;
using Moq;
using PropFinderApi.Exceptions;
using PropFinderApi.Misc;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Services;

namespace PropFinderApi.Tests.Services
{
    [TestFixture]
    public class ContactServiceTests
    {
        private Mock<IContactLogRepository> _contactLogRepoMock;
        private Mock<IUserRepository> _userRepoMock;
        private Mock<IListerProfileRepository> _listerProfileRepoMock;
        private Mock<IRepository<Guid, Property>> _propertyRepoMock;
        private Mock<IHubContext<NotificationHub>> _hubContextMock;
        private Mock<IClientProxy> _clientProxyMock;
        private Mock<IHubClients> _hubClientsMock;

        private ContactService _contactService;

        [SetUp]
        public void SetUp()
        {
            _contactLogRepoMock = new Mock<IContactLogRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _listerProfileRepoMock = new Mock<IListerProfileRepository>();
            _propertyRepoMock = new Mock<IRepository<Guid, Property>>();
            _hubContextMock = new Mock<IHubContext<NotificationHub>>();
            _clientProxyMock = new Mock<IClientProxy>();
            _hubClientsMock = new Mock<IHubClients>();

            _hubContextMock.Setup(h => h.Clients).Returns(_hubClientsMock.Object);
            _hubClientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(_clientProxyMock.Object);

            _contactService = new ContactService(
                _contactLogRepoMock.Object,
                _userRepoMock.Object,
                _listerProfileRepoMock.Object,
                _propertyRepoMock.Object,
                _hubContextMock.Object
            );
        }

        [Test]
        public async Task ContactListerAsync_ValidRequest_ReturnsListerContact()
        {
            var propertyId = Guid.NewGuid();
            var buyerId = Guid.NewGuid();
            var listerId = Guid.NewGuid();

            var buyer = new User
            {
                Id = buyerId,
                Name = "Buyer",
                PhoneNumber = ""
            };
            var lister = new User
            {
                Id = listerId,
                Name = "Lister",
                Email = "lister@example.com"
            };
            var listerProfile = new ListerProfile
            {
                UserId = listerId,
                BusinessPhoneNumber = "9999999999"
            };
            var property = new Property
            {
                Id = propertyId,
                Title = "Flat 101",
                ListerId = listerId
            };

            var request = new ContactListerRequestDto
            {
                PropertyId = propertyId,
                BuyerPhoneNumber = "8888888888",
                BuyerEmail = "buyer@example.com",
                Message = "I'm interested"
            };

            _userRepoMock.Setup(r => r.Get(buyerId)).ReturnsAsync(buyer);
            _propertyRepoMock.Setup(r => r.Get(propertyId)).ReturnsAsync(property);
            _userRepoMock.Setup(r => r.Get(listerId)).ReturnsAsync(lister);
            _listerProfileRepoMock.Setup(r => r.GetByUserIdAsync(listerId)).ReturnsAsync(listerProfile);

            ContactLog? savedLog = null;

            _contactLogRepoMock
                .Setup(r => r.Add(It.IsAny<ContactLog>()))
                .Callback<ContactLog>(log => savedLog = log)
                .ReturnsAsync((ContactLog log) => log);

            var result = await _contactService.ContactListerAsync(request, buyerId);

            Assert.IsNotNull(result);
            Assert.That(result.ListerEmail, Is.EqualTo("lister@example.com"));
            Assert.That(result.ListerPhoneNumber, Is.EqualTo("9999999999"));

            Assert.IsNotNull(savedLog);
            Assert.That(savedLog.BuyerPhoneNumber, Is.EqualTo("8888888888"));
            Assert.That(savedLog.BuyerEmail, Is.EqualTo("buyer@example.com"));
            Assert.That(savedLog.Message, Is.EqualTo("I'm interested"));
            Assert.That(savedLog.ListerName, Is.EqualTo("Lister"));

            _clientProxyMock.Verify(p => p.SendCoreAsync(
                "NewInquiryReceived",
                It.IsAny<object[]>(),
                default), Times.Once);
        }


        [Test]
        public void ContactListerAsync_InvalidProperty_ThrowsBadRequest()
        {
            var buyerId = Guid.NewGuid();
            var listerId = Guid.NewGuid();

            var buyer = new User
            {
                Id = buyerId,
                Name = "Buyer",
                PhoneNumber = ""
            };
            var dto = new ContactListerRequestDto
            {
                PropertyId = Guid.Empty,
                BuyerPhoneNumber = "1234567890",
                BuyerEmail = "buyer@example.com",
                Message = "Interested"
            };

            var ex = Assert.ThrowsAsync<BadRequestException>(() =>
                _contactService.ContactListerAsync(dto, buyerId));

            Assert.That(ex.Message, Is.EqualTo("Invalid property ID."));
        }

        [Test]
        public async Task GetContactLogsForPropertyAsync_AsValidLister_ReturnsLogs()
        {
            var propertyId = Guid.NewGuid();
            var listerId = Guid.NewGuid();
            var requesterId = listerId;

            var property = new Property
            {
                Id = propertyId,
                ListerId = listerId
            };
            var contactLogs = new List<ContactLog>
            {
                new ContactLog
                {
                    Property = new Property { Id = propertyId, Title = "Villa", Location = new PropertyLocation { City = "Chennai", Locality = "Adyar" }},
                    Message = "Interested",
                    BuyerEmail = "buyer@example.com",
                    BuyerPhoneNumber = "9876543210",
                    CreatedAt = DateTime.UtcNow
                }
            };

            _propertyRepoMock.Setup(r => r.Get(propertyId)).ReturnsAsync(property);
            _contactLogRepoMock.Setup(r => r.GetByPropertyIdAsync(propertyId)).ReturnsAsync(contactLogs);

            var result = await _contactService.GetContactLogsForPropertyAsync(propertyId, requesterId, "Lister");

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Location, Is.EqualTo("Adyar, Chennai"));
        }

        [Test]
        public void GetContactLogsForPropertyAsync_WrongLister_ThrowsUnauthorized()
        {
            var propertyId = Guid.NewGuid();
            var listerId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();

            var property = new Property
            {
                Id = propertyId,
                ListerId = listerId
            };

            _propertyRepoMock.Setup(r => r.Get(propertyId)).ReturnsAsync(property);

            var ex = Assert.ThrowsAsync<UnauthorizedException>(() =>
                _contactService.GetContactLogsForPropertyAsync(propertyId, requesterId, "Lister"));

            Assert.That(ex.Message, Is.EqualTo("You can view only your contact logs"));
        }

        [Test]
        public async Task GetContactLogsForListerAsync_ValidLister_ReturnsLogs()
        {
            var listerId = Guid.NewGuid();
            var requesterId = listerId;

            var logs = new List<ContactLog>
            {
                new ContactLog
                {
                    Property = new Property { Title = "Home", Location = new PropertyLocation { City = "Pune", Locality = "Wakad" }},
                    Message = "Hi", CreatedAt = DateTime.UtcNow,
                    BuyerEmail = "buyer@test.com", BuyerPhoneNumber = "9000000000"
                }
            };

            _contactLogRepoMock.Setup(r => r.GetByListerIdAsync(listerId)).ReturnsAsync(logs);

            var result = await _contactService.GetContactLogsForListerAsync(listerId, requesterId, "Lister");

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Location, Is.EqualTo("Wakad, Pune"));
        }

        [Test]
        public async Task GetContactLogsForListerAsync_AsAdmin_ReturnsLogs()
        {
            var listerId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();

            var logs = new List<ContactLog>
            {
                new ContactLog
                {
                    Property = new Property { Title = "Home", Location = new PropertyLocation { City = "Pune", Locality = "Wakad" }},
                    Message = "Hi", CreatedAt = DateTime.UtcNow,
                    BuyerEmail = "buyer@test.com", BuyerPhoneNumber = "9000000000"
                }
            };

            _contactLogRepoMock.Setup(r => r.GetByListerIdAsync(listerId)).ReturnsAsync(logs);

            var result = await _contactService.GetContactLogsForListerAsync(listerId, requesterId, "Admin");

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Location, Is.EqualTo("Wakad, Pune"));
        }

        [Test]
        public void GetContactLogsForListerAsync_AsOtherLister_Unauthorized_ThrowsException()
        {
            var listerId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();

            var ex = Assert.ThrowsAsync<UnauthorizedException>(() =>
                _contactService.GetContactLogsForListerAsync(listerId, requesterId, "Lister"));

            Assert.That(ex.Message, Is.EqualTo("You can view only your contact logs"));
        }

        [Test]
        public async Task GetContactLogsForBuyerAsync_ValidBuyer_ReturnsLogs()
        {
            var buyerId = Guid.NewGuid();
            var requesterId = buyerId;

            var logs = new List<ContactLog>
            {
                new ContactLog
                {
                    Property = new Property { Title = "House", Location = new PropertyLocation { City = "Delhi", Locality = "Karol Bagh" }},
                    Message = "Is this available?", CreatedAt = DateTime.UtcNow,
                    ListerName = "Ram", ListerEmail = "ram@test.com", ListerPhoneNumber = "7000000000"
                }
            };

            _contactLogRepoMock.Setup(r => r.GetByBuyerIdAsync(buyerId)).ReturnsAsync(logs);

            var result = await _contactService.GetContactLogsForBuyerAsync(buyerId,requesterId, "Buyer");

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Location, Is.EqualTo("Karol Bagh, Delhi"));
            Assert.That(result.First().ListerName, Is.EqualTo("Ram"));
        }

        [Test]
        public void GetContactLogsForBuyerAsync_Unauthorized_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<UnauthorizedException>(() =>
                _contactService.GetContactLogsForBuyerAsync(Guid.NewGuid(), Guid.NewGuid(), "Buyer"));

            Assert.That(ex.Message, Is.EqualTo("You can view only your contact logs"));
        }
    }
}
