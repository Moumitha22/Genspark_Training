// using System;
// using System.Collections.Generic;
// using System.Security.Claims;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using NUnit.Framework;
// using PropFinderApi.Controllers;
// using PropFinderApi.Exceptions;
// using PropFinderApi.Interfaces;
// using PropFinderApi.Models;
// using PropFinderApi.Models.DTOs;

// namespace PropFinderApi.Tests.Controllers
// {
//     public class ContactControllerTests
//     {
//         private Mock<IContactLogService> _contactServiceMock;
//         private Mock<IPaginationService> _paginationServiceMock;
//         private Mock<IApiResponseMapper> _mapperMock;
//         private ContactController _controller;

//         [SetUp]
//         public void Setup()
//         {
//             _contactServiceMock = new Mock<IContactLogService>();
//             _paginationServiceMock = new Mock<IPaginationService>();
//             _mapperMock = new Mock<IApiResponseMapper>();

//             _controller = new ContactController(
//                 _contactServiceMock.Object,
//                 _paginationServiceMock.Object,
//                 _mapperMock.Object
//             );
//         }

//         private void SetUserContext(Guid userId, string role)
//         {
//             var claims = new List<Claim>
//             {
//                 new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
//                 new Claim(ClaimTypes.Role, role)
//             };

//             var identity = new ClaimsIdentity(claims, "TestAuthType");
//             var principal = new ClaimsPrincipal(identity);

//             _controller.ControllerContext = new ControllerContext
//             {
//                 HttpContext = new DefaultHttpContext { User = principal }
//             };
//         }

//         [Test]
//         public async Task ContactLister_ReturnsOk()
//         {
//             var buyerId = Guid.NewGuid();
//             SetUserContext(buyerId, "Buyer");

//             var requestDto = new ContactListerRequestDto
//             {
//                 PropertyId = Guid.NewGuid(),
//                 BuyerPhoneNumber = "9999999999"
//             };

//             var contactResponse = new ContactListerResponseDto
//             {
//                 ListerEmail = "agent@gmail.com",
//                 ListerPhoneNumber = "9876543212"
//             };

//             _contactServiceMock.Setup(s => s.ContactListerAsync(requestDto, buyerId))
//                 .ReturnsAsync(contactResponse);

//             var result = await _controller.ContactLister(requestDto);

//             Assert.That(result, Is.InstanceOf<OkObjectResult>());
//         }

//         [Test]
//         public void ContactLister_ThrowsBadRequest_IfPropertyIdEmpty()
//         {
//             SetUserContext(Guid.NewGuid(), "Buyer");

//             var dto = new ContactListerRequestDto
//             {
//                 PropertyId = Guid.Empty,
//                 BuyerPhoneNumber = "8888888888"
//             };

//             Assert.ThrowsAsync<BadRequestException>(() => _controller.ContactLister(dto));
//         }

//         [Test]
//         public async Task GetListerContactLogs_ReturnsOk()
//         {
//             var listerId = Guid.NewGuid();
//             var requesterId = listerId;
//             SetUserContext(requesterId, "Lister");

//             var logs = new List<ContactLog>
//             {
//                 new ContactLog { Id = Guid.NewGuid(), ListerId = listerId, BuyerId = Guid.NewGuid() }
//             };

//             var paginated = new List<ContactLog>(logs);
//             var pagination = new PaginationInfoDto { TotalItems = 1, CurrentPage = 1, PageSize = 10, TotalPages = 1 };

//             _contactServiceMock.Setup(s => s.GetContactLogsForListerAsync(listerId, requesterId, "Lister")).ReturnsAsync(logs);
//             _paginationServiceMock.Setup(p => p.ApplyPagination(logs, 1, 10)).Returns((paginated, pagination));
//             _mapperMock.Setup(m => m.MapToOkResponse("Contact logs fetched successfully", paginated, pagination))
//                        .Returns(new ApiResponse<List<ContactLog>>());

//             var result = await _controller.GetListerContactLogs(listerId, 1, 10);

//             Assert.That(result, Is.InstanceOf<OkObjectResult>());
//         }

//         [Test]
//         public async Task GetBuyerContactLogs_ReturnsOk()
//         {
//             var buyerId = Guid.NewGuid();
//             SetUserContext(Guid.NewGuid(), "Admin");

//             var logs = new List<ContactLog>
//             {
//                 new ContactLog { Id = Guid.NewGuid(), listerId = Guid.NewGuid(), BuyerId = buyerId }
//             };

//             var paginated = new List<ContactLog>(logs);
//             var pagination = new PaginationInfoDto { TotalItems = 1, CurrentPage = 1, PageSize = 10, TotalPages = 1 };

//             _contactServiceMock.Setup(s => s.GetContactLogsForBuyerAsync(buyerId, It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(logs);
//             _paginationServiceMock.Setup(p => p.ApplyPagination(logs, 1, 10)).Returns((paginated, pagination));
//             _mapperMock.Setup(m => m.MapToOkResponse("All contact logs fetched successfully", paginated, pagination))
//                        .Returns(new ApiResponse<List<ContactLog>>());

//             var result = await _controller.GetBuyerContactLogs(buyerId, 1, 10);

//             Assert.That(result, Is.InstanceOf<OkObjectResult>());
//         }
//     }
// }
