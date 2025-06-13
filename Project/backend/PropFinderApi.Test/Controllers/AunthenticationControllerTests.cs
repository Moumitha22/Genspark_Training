using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PropFinderApi.Controllers;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Tests.Controllers
{
    public class AuthenticationControllerTests
    {
        private Mock<IAuthenticationService> _authServiceMock;
        private Mock<IApiResponseMapper> _responseMapperMock;
        private AuthenticationController _controller;

        [SetUp]
        public void SetUp()
        {
            _authServiceMock = new Mock<IAuthenticationService>();
            _responseMapperMock = new Mock<IApiResponseMapper>();
            _controller = new AuthenticationController(_authServiceMock.Object, _responseMapperMock.Object);
        }

        [Test]
        public async Task Register_ReturnsOkResponse()
        {
            var request = new UserRegisterRequestDto
            {
                Email = "test@example.com",
                Password = "Test123"
            };
            var serviceResult = "User registered successfully";
            var expectedResponse = new ApiResponse<string> { Message = "User registered successfully", Data = serviceResult };

            _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(serviceResult);
            _responseMapperMock.Setup(r => r.MapToOkResponse("User registered successfully", serviceResult)).Returns(expectedResponse);

            var result = await _controller.Register(request) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(expectedResponse));
        }

        [Test]
        public async Task Login_ReturnsLoginResponseDto()
        {
            var request = new UserLoginRequestDto { Email = "test@example.com", Password = "Test123" };
            var loginResponse = new UserLoginResponseDto { AccessToken = "token", RefreshToken = "refresh" };
            var expectedResponse = new ApiResponse<UserLoginResponseDto>
            {
                Message = "Login successful",
                Data = loginResponse
            };

            _authServiceMock.Setup(s => s.LoginAsync(request)).ReturnsAsync(loginResponse);
            _responseMapperMock.Setup(r => r.MapToOkResponse("Login successful", loginResponse)).Returns(expectedResponse);

            var result = await _controller.Login(request) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(expectedResponse));
        }

        [Test]
        public async Task RefreshToken_ReturnsNewTokens()
        {
            var request = new RefreshTokenRequestDto { RefreshToken = "refresh-token" };
            var loginResponse = new UserLoginResponseDto { AccessToken = "new-token", RefreshToken = "new-refresh" };
            var expectedResponse = new ApiResponse<UserLoginResponseDto>
            {
                Message = "Token refreshed successfully",
                Data = loginResponse
            };

            _authServiceMock.Setup(s => s.RefreshTokenAsync("refresh-token")).ReturnsAsync(loginResponse);
            _responseMapperMock.Setup(r => r.MapToOkResponse("Token refreshed successfully", loginResponse)).Returns(expectedResponse);

            var result = await _controller.RefreshToken(request) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(expectedResponse));
        }

        [Test]
        public async Task Logout_ReturnsSuccessMessage()
        {
            var request = new RefreshTokenRequestDto { RefreshToken = "refresh-token" };
            var expectedResponse = new ApiResponse<object> { Message = "Logged out successfully" };

            _authServiceMock.Setup(s => s.LogoutAsync("refresh-token")).Returns(Task.CompletedTask);
            _responseMapperMock.Setup(r => r.MapToOkResponse("Logged out successfully")).Returns(expectedResponse);

            var result = await _controller.Logout(request) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(expectedResponse));
        }
    }
}
