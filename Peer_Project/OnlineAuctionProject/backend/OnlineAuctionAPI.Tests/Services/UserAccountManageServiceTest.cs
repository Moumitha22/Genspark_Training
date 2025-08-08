using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using OnlineAuctionAPI.Interfaces.Repository;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;
using OnlineAuctionAPI.Services;

namespace OnlineAuctionAPI.Tests.Service
{
    public class UserAccountManageServiceTests
    {
        private Mock<IUserAccountManageRepository> _mockRepo;
        private UserAccountManageService _service;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IUserAccountManageRepository>();
            _service = new UserAccountManageService(_mockRepo.Object);
        }

        [Test]
        public async Task RestoreUserByEmailAsync_CallsRepository()
        {
            var email = "test@example.com";
            _mockRepo.Setup(r => r.RestoreUserByEmailAsync(email)).Returns(Task.CompletedTask);

            await _service.RestoreUserByEmailAsync(email);

            _mockRepo.Verify(r => r.RestoreUserByEmailAsync(email), Times.Once);
        }

        [Test]
        public async Task SoftDeleteUserAsync_CallsRepository()
        {
            var dto = new UserDeleteRequest { UserId = Guid.NewGuid(), Reason = "Test reason" };
            _mockRepo.Setup(r => r.SoftDeleteUserAsync(dto)).Returns(Task.CompletedTask);

            await _service.SoftDeleteUserAsync(dto);

            _mockRepo.Verify(r => r.SoftDeleteUserAsync(dto), Times.Once);
        }

        [Test]
        public async Task GetDeleteReasonByEmail_ReturnsReason()
        {
            var email = "test@example.com";
            _mockRepo.Setup(r => r.GetDeleteReasonByEmailAsync(email)).ReturnsAsync("Reason");

            var result = await _service.GetDeleteReasonByEmail(email);

            Assert.AreEqual("Reason", result);
        }

        [Test]
        public async Task GetAllDeletedUsersAsync_ReturnsList()
        {
            var deletedUsers = new List<DeletedUserDto> { new DeletedUserDto() };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(deletedUsers);

            var result = await _service.GetAllDeletedUsersAsync();

            Assert.AreEqual(deletedUsers, result);
        }
    }
}
