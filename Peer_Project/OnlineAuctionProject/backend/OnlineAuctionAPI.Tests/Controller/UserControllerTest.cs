using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OnlineAuctionAPI.Controllers;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Tests.Controller
{
    [TestFixture]
    public class UserControllerTest
    {
        private Mock<IUserService> _mockUserService;
        private Mock<ILogger<UserController>> _mockLogger;
        private Mock<IVirtualWalletService> _mockVirtualWalletService;
        private Mock<Azure.Security.KeyVault.Secrets.SecretClient> _mockSecretClient;

        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<UserController>>();
            _mockVirtualWalletService = new Mock<IVirtualWalletService>();
            _mockSecretClient = new Mock<Azure.Security.KeyVault.Secrets.SecretClient>();

            _controller = new UserController(
                _mockUserService.Object,
                _mockLogger.Object,
                _mockVirtualWalletService.Object,
                _mockSecretClient.Object
            );
        }

        [Test]
        public async Task GetUserById_ReturnsOkWithUser()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Password = "dummy",
            };
            _mockUserService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(user);

            var actionResult = await _controller.GetUserById(userId);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var apiResponse = okResult.Value as ApiResponse<User>;
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(user, apiResponse.Data);
        }

        [Test]
        public async Task GetUserById_Exception_ThrowsException()
        {
            var userId = Guid.NewGuid();
            _mockUserService
                .Setup(s => s.GetUserByIdAsync(userId))
                .ThrowsAsync(new Exception("DB error"));

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _controller.GetUserById(userId)
            );
            Assert.That(ex.Message, Does.Contain("DB error"));
        }

        [Test]
        public async Task GetUserById_NotFound_ReturnsNotFound()
        {
            var userId = Guid.NewGuid();
            _mockUserService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            var actionResult = await _controller.GetUserById(userId);
            var notFoundResult = actionResult.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);

            var apiResponse = notFoundResult.Value as ApiResponse<string>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("User not found", apiResponse.Message);
        }

        [Test]
        public async Task GetByEmail_ReturnsOkWithUser()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Password = "dummy",
            };
            _mockUserService.Setup(s => s.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);

            var actionResult = await _controller.GetByEmail(user.Email);
            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            var apiResponse = okResult.Value as ApiResponse<User>;
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(user, apiResponse.Data);
        }

        [Test]
        public async Task GetByEmail_Exception_ThrowsException()
        {
            var email = "test@example.com";
            _mockUserService
                .Setup(s => s.GetUserByEmailAsync(email))
                .ThrowsAsync(new Exception("Email error"));

            var ex = Assert.ThrowsAsync<Exception>(async () => await _controller.GetByEmail(email));
            Assert.That(ex.Message, Does.Contain("Email error"));
        }

        [Test]
        public async Task CreateUser_ReturnsCreatedAtAction()
        {
            var userDto = new UserRegisterRequestDto
            {
                Email = "test@example.com",
                Password = "password123",
                UserName = "testuser",
                Role = "Bidder",
            };
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Password = "dummy",
            };
            _mockUserService.Setup(s => s.CreateUserAsync(userDto)).ReturnsAsync(user);

            var actionResult = await _controller.CreateUser(userDto);
            var createdResult = actionResult.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            var apiResponse = createdResult.Value as ApiResponse<User>;
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(user, apiResponse.Data);
        }

        [Test]
        public async Task CreateUser_Exception_ThrowsException()
        {
            var userDto = new UserRegisterRequestDto
            {
                Email = "test@example.com",
                Password = "password123",
                UserName = "testuser",
                Role = "Bidder",
            };
            _mockUserService
                .Setup(s => s.CreateUserAsync(userDto))
                .ThrowsAsync(new Exception("Create error"));

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _controller.CreateUser(userDto)
            );
            Assert.That(ex.Message, Does.Contain("Create error"));
        }

        [Test]
        public async Task UpdateUser_ReturnsOkWithUser()
        {
            var userId = Guid.NewGuid();
            var updateDto = new UserUpdateRequestDto
            {
                UserName = "updateduser",
                Email = "updated@example.com",
            };
            var updatedUser = new User
            {
                Id = userId,
                Username = "updateduser",
                Email = "updated@example.com",
                Password = "dummy",
            };
            _mockUserService
                .Setup(s => s.UpdateUserInfoAsync(userId, updateDto))
                .ReturnsAsync(updatedUser);

            var actionResult = await _controller.Updateuser(userId, updateDto);
            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            var apiResponse = okResult.Value as ApiResponse<User>;
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(updatedUser, apiResponse.Data);
        }

        [Test]
        public async Task UpdateUser_Exception_ReturnsBadRequest()
        {
            var userId = Guid.NewGuid();
            var updateDto = new UserUpdateRequestDto
            {
                UserName = "updateduser",
                Email = "updated@example.com",
            };
            _mockUserService
                .Setup(s => s.UpdateUserInfoAsync(userId, updateDto))
                .ThrowsAsync(new Exception("Update error"));

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _controller.Updateuser(userId, updateDto)
            );
            Assert.That(ex.Message, Does.Contain("Update error"));
        }
    }
}
