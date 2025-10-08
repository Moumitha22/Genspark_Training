using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
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
    public class ContactControllerTests
    {
        private Mock<IContactLogService> _contactServiceMock;
        private Mock<IPaginationService> _paginationServiceMock;
        private Mock<IApiResponseMapper> _mapperMock;
        private ContactController _controller;

        [SetUp]
        public void Setup()
        {
            _contactServiceMock = new Mock<IContactLogService>();
            _paginationServiceMock = new Mock<IPaginationService>();
            _mapperMock = new Mock<IApiResponseMapper>();

            _controller = new ContactController(
                _contactServiceMock.Object,
                _paginationServiceMock.Object,
                _mapperMock.Object
            );
        }

        private void SetUserContext(Guid userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Test]
        public async Task ContactAgent_ReturnsOk()
        {
            var buyerId = Guid.NewGuid();
            SetUserContext(buyerId, "Buyer");

            var requestDto = new ContactAgentRequestDto
            {
                PropertyId = Guid.NewGuid(),
                BuyerPhoneNumber = "9999999999"
            };

            var contactResponse = new ContactAgentResponseDto
            {
                AgentEmail = "agent@gmail.com",
                AgentPhoneNumber = "9876543212"
            };

            _contactServiceMock.Setup(s => s.ContactAgentAsync(requestDto, buyerId))
                .ReturnsAsync(contactResponse);

            var result = await _controller.ContactAgent(requestDto);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void ContactAgent_ThrowsBadRequest_IfPropertyIdEmpty()
        {
            SetUserContext(Guid.NewGuid(), "Buyer");

            var dto = new ContactAgentRequestDto
            {
                PropertyId = Guid.Empty,
                BuyerPhoneNumber = "8888888888"
            };

            Assert.ThrowsAsync<BadRequestException>(() => _controller.ContactAgent(dto));
        }

        [Test]
        public async Task GetAgentContactLogs_ReturnsOk()
        {
            var agentId = Guid.NewGuid();
            var requesterId = agentId;
            SetUserContext(requesterId, "Agent");

            var logs = new List<ContactLog>
            {
                new ContactLog { Id = Guid.NewGuid(), AgentId = agentId, BuyerId = Guid.NewGuid() }
            };

            var paginated = new List<ContactLog>(logs);
            var pagination = new PaginationInfoDto { TotalItems = 1, CurrentPage = 1, PageSize = 10, TotalPages = 1 };

            _contactServiceMock.Setup(s => s.GetContactLogsForAgentAsync(agentId, requesterId, "Agent")).ReturnsAsync(logs);
            _paginationServiceMock.Setup(p => p.ApplyPagination(logs, 1, 10)).Returns((paginated, pagination));
            _mapperMock.Setup(m => m.MapToOkResponse("Contact logs fetched successfully", paginated, pagination))
                       .Returns(new ApiResponse<List<ContactLog>>());

            var result = await _controller.GetAgentContactLogs(agentId, 1, 10);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetBuyerContactLogs_ReturnsOk()
        {
            var buyerId = Guid.NewGuid();
            SetUserContext(Guid.NewGuid(), "Admin");

            var logs = new List<ContactLog>
            {
                new ContactLog { Id = Guid.NewGuid(), AgentId = Guid.NewGuid(), BuyerId = buyerId }
            };

            var paginated = new List<ContactLog>(logs);
            var pagination = new PaginationInfoDto { TotalItems = 1, CurrentPage = 1, PageSize = 10, TotalPages = 1 };

            _contactServiceMock.Setup(s => s.GetContactLogsForBuyerAsync(buyerId, It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(logs);
            _paginationServiceMock.Setup(p => p.ApplyPagination(logs, 1, 10)).Returns((paginated, pagination));
            _mapperMock.Setup(m => m.MapToOkResponse("All contact logs fetched successfully", paginated, pagination))
                       .Returns(new ApiResponse<List<ContactLog>>());

            var result = await _controller.GetBuyerContactLogs(buyerId, 1, 10);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
    }
}
