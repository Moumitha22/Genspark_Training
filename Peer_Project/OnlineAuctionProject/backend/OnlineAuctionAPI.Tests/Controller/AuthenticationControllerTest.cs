using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OnlineAuctionAPI.Controllers;
using OnlineAuctionAPI.Hubs;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Tests.Controller
{
    [TestFixture]
    public class AuthenticationControllerTest
    {
        private Mock<IAuthService> _mockAuthService;
        private Mock<ITokenService> _mockTokenService;
        private Mock<ILogger<AuthenticationController>> _mockLogger;
        private AuthenticationController _controller;

        [SetUp]
        public void Setup()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockTokenService = new Mock<ITokenService>();
            _mockLogger = new Mock<ILogger<AuthenticationController>>();
            _controller = new AuthenticationController(
                _mockAuthService.Object,
                _mockTokenService.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task UserLogin_ReturnsOkWithUserLoginResponse()
        {
            var loginRequest = new UserLoginRequestDto
            {
                Email = "test@example.com",
                Password = "password123",
            };
            var loginResponse = new UserLoginResponseDto
            {
                UserName = "testuser",
                Email = "test@example.com",
                Token = "access_token",
                RefreshToken = "refresh_token",
            };
            _mockAuthService.Setup(s => s.LoginAsync(loginRequest)).ReturnsAsync(loginResponse);

            var actionResult = await _controller.UserLogin(loginRequest);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var apiResponse = okResult.Value as ApiResponse<UserLoginResponseDto>;
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(loginResponse, apiResponse.Data);
        }

        [Test]
        public async Task Logout_ReturnsOk()
        {
            var refreshToken = "token";
            _mockAuthService.Setup(s => s.LogoutAsync(refreshToken)).Returns(Task.CompletedTask);

            var actionResult = await _controller.Logout(refreshToken);
            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            var apiResponse = okResult.Value as ApiResponse<string>;
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual("Logout successful", apiResponse.Message);
        }

        [Test]
        public async Task RefreshToken_ValidToken_ReturnsOkWithUserLoginResponse()
        {
            var refreshToken = "token";
            var loginResponse = new UserLoginResponseDto
            {
                UserName = "testuser",
                Email = "test@example.com",
                Token = "access_token",
                RefreshToken = "refresh_token",
            };
            _mockTokenService
                .Setup(s => s.RefreshTokenAsync(refreshToken))
                .ReturnsAsync(loginResponse);

            var actionResult = await _controller.RefreshToken(refreshToken);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var apiResponse = okResult.Value as ApiResponse<UserLoginResponseDto>;
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(loginResponse, apiResponse.Data);
        }

        [Test]
        public async Task RefreshToken_InvalidToken_ReturnsUnauthorized()
        {
            var refreshToken = "badtoken";
            _mockTokenService
                .Setup(s => s.RefreshTokenAsync(refreshToken))
                .ReturnsAsync((UserLoginResponseDto?)null);

            var actionResult = await _controller.RefreshToken(refreshToken);
            var unauthorizedResult = actionResult.Result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);

            var apiResponse = unauthorizedResult.Value as ApiResponse<string>;
            Assert.IsNotNull(apiResponse);
            Assert.That(apiResponse.Message, Is.EqualTo("Invalid refresh token"));
        }

        [Test]
        public async Task RefreshToken_Exception_ThrowsException()
        {
            var refreshToken = "anytoken";
            _mockTokenService
                .Setup(s => s.RefreshTokenAsync(refreshToken))
                .ThrowsAsync(new System.Exception("Something went wrong"));

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _controller.RefreshToken(refreshToken)
            );
            Assert.That(ex.Message, Does.Contain("Something went wrong"));
        }
    }
}
