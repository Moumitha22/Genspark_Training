using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PropFinderApi.Controllers;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Tests.Controllers
{
    public class UserControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private Mock<IPaginationService> _paginationServiceMock;
        private Mock<IApiResponseMapper> _mapperMock;
        private Mock<ILogger<UserController>> _loggerMock;
        private UserController _controller;

        [SetUp]
        public void SetUp()
        {
            _userServiceMock = new Mock<IUserService>();
            _paginationServiceMock = new Mock<IPaginationService>();
            _mapperMock = new Mock<IApiResponseMapper>();
            _loggerMock = new Mock<ILogger<UserController>>();

            _controller = new UserController(
                _userServiceMock.Object,
                _paginationServiceMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        private void SetUserContext(Guid userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Test]
        public async Task GetAllUsers_ReturnsOk()
        {
            SetUserContext(Guid.NewGuid(), "Admin");

            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = "john@example.com", Name = "John" },
                new User { Id = Guid.NewGuid(), Email = "dia@example.com", Name = "Dia" },
            };

            var pagination = new PaginationInfoDto { PageSize = 10, TotalItems = 2, TotalPages = 1, CurrentPage = 1 };
            var response = new ApiResponse<List<User>>();

            _userServiceMock.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(users);
            _paginationServiceMock.Setup(x => x.ApplyPagination(users, 1, 10)).Returns((users, pagination));
            _mapperMock.Setup(m => m.MapToOkResponse("Users fetched successfully", users, pagination)).Returns(response);

            var result = await _controller.GetAllUsers(1, 10);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetUserById_ReturnsOk()
        {
            var id = Guid.NewGuid();
            SetUserContext(Guid.NewGuid(), "Admin");

            var user = new User { Id = id, Email = "jane@example.com", Name = "Jane" };
            var response = new ApiResponse<User>();

            _userServiceMock.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync(user);
            _mapperMock.Setup(m => m.MapToOkResponse("User fetched successfully", user)).Returns(response);

            var result = await _controller.GetUserById(id);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetUserByEmail_ReturnsOk()
        {
            SetUserContext(Guid.NewGuid(), "Admin");

            var email = "test@example.com";
            var user = new User { Id = Guid.NewGuid(), Email = email, Name = "Test" };
            var response = new ApiResponse<User>();

            _userServiceMock.Setup(s => s.GetUserByEmailAsync(email)).ReturnsAsync(user);
            _mapperMock.Setup(m => m.MapToOkResponse("User fetched successfully", user)).Returns(response);

            var result = await _controller.GetUserByEmail(email);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void GetUserByEmail_ThrowsNotFound_WhenUserNull()
        {
            var email = "notfound@example.com";
            SetUserContext(Guid.NewGuid(), "Agent");

            _userServiceMock.Setup(s => s.GetUserByEmailAsync(email)).ReturnsAsync((User)null);

            Assert.ThrowsAsync<NotFoundException>(() => _controller.GetUserByEmail(email));
        }

        [Test]
        public async Task UpdateUser_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var dto = new UserUpdateRequestDto { Name = "Updated User" };

            var updatedUser = new User { Id = userId, Name = "Updated User", Email = "updated@example.com" };
            var response = new ApiResponse<User>();

            SetUserContext(requesterId, "Admin");

            _userServiceMock.Setup(s => s.UpdateUserAsync(userId, dto, requesterId, "Admin"))
                            .ReturnsAsync(updatedUser);
            _mapperMock.Setup(m => m.MapToOkResponse("User updated successfully", updatedUser)).Returns(response);

            var result = await _controller.UpdateUser(userId, dto);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task DeleteUser_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            SetUserContext(Guid.NewGuid(), "Admin");

            var response = new ApiResponse<object>();
            _userServiceMock.Setup(s => s.DeleteUserAsync(userId)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.MapToOkResponse("User deleted successfully")).Returns(response);

            var result = await _controller.DeleteUser(userId);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void DeleteUser_ThrowsBadRequest_WhenIdEmpty()
        {
            SetUserContext(Guid.NewGuid(), "Admin");

            Assert.ThrowsAsync<BadRequestException>(() => _controller.DeleteUser(Guid.Empty));
        }
    }
}
