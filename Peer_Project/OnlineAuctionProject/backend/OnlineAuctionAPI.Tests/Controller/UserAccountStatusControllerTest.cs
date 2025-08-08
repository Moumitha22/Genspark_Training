using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OnlineAuctionAPI.Controllers;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Tests.Controllers
{
    public class UserAccountStatusControllerTests
    {
        private Mock<IUserAccountManageService> _mockService;
        private Mock<ILogger<UserAccountStatusController>> _mockLogger;
        private UserAccountStatusController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IUserAccountManageService>();
            _mockLogger = new Mock<ILogger<UserAccountStatusController>>();
            _controller = new UserAccountStatusController(_mockService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task DeleteUser_ReturnsOk()
        {
            var dto = new UserDeleteRequest { UserId = Guid.NewGuid(), Reason = "Test" };
            _mockService.Setup(s => s.SoftDeleteUserAsync(dto)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteUser(dto);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task DeleteUser_Exception_Returns500()
        {
            var dto = new UserDeleteRequest { UserId = Guid.NewGuid(), Reason = "Test" };
            _mockService
                .Setup(s => s.SoftDeleteUserAsync(dto))
                .ThrowsAsync(new Exception("Delete error"));

            try
            {
                await _controller.DeleteUser(dto);
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Delete error", ex.Message);
            }
        }

        [Test]
        public async Task RestoreUserByEmail_ReturnsOk()
        {
            var email = "test@example.com";
            _mockService.Setup(s => s.RestoreUserByEmailAsync(email)).Returns(Task.CompletedTask);

            var result = await _controller.RestoreUserByEmail(email);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task RestoreUserByEmail_Exception_Returns500()
        {
            var email = "test@example.com";
            _mockService
                .Setup(s => s.RestoreUserByEmailAsync(email))
                .ThrowsAsync(new Exception("Restore error"));

            try
            {
                await _controller.RestoreUserByEmail(email);
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Restore error", ex.Message);
            }
        }

        [Test]
        public async Task GetDeleteReasonByEmail_ReturnsOk()
        {
            var email = "test@example.com";
            _mockService.Setup(s => s.GetDeleteReasonByEmail(email)).ReturnsAsync("Reason");

            var result = await _controller.GetDeleteReasonByEmail(email);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetDeleteReasonByEmail_NotFound_ReturnsNotFound()
        {
            var email = "test@example.com";
            _mockService.Setup(s => s.GetDeleteReasonByEmail(email)).ReturnsAsync((string)null);

            var result = await _controller.GetDeleteReasonByEmail(email);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task GetAllDeletedUsers_ReturnsOk()
        {
            var deletedUsers = new List<DeletedUserDto> { new DeletedUserDto() };
            _mockService.Setup(s => s.GetAllDeletedUsersAsync()).ReturnsAsync(deletedUsers);

            var result = await _controller.GetAllDeletedUsers();

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetAllDeletedUsers_NotFound_ReturnsNotFound()
        {
            _mockService
                .Setup(s => s.GetAllDeletedUsersAsync())
                .ReturnsAsync((List<DeletedUserDto>)null);

            var result = await _controller.GetAllDeletedUsers();

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
    }
}
