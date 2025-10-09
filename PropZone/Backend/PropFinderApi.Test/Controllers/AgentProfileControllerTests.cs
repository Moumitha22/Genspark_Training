// using System.Security.Claims;
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
//     [TestFixture]
//     public class ListerProfileControllerTests
//     {
//         private Mock<IListerProfileService> _listerProfileServiceMock;
//         private Mock<IPaginationService> _paginationServiceMock;
//         private Mock<IApiResponseMapper> _responseMapperMock;
//         private ListerProfileController _controller;

//         [SetUp]
//         public void Setup()
//         {
//             _listerProfileServiceMock = new Mock<IListerProfileService>();
//             _paginationServiceMock = new Mock<IPaginationService>();
//             _responseMapperMock = new Mock<IApiResponseMapper>();
//             _controller = new ListerProfileController(
//                 _listerProfileServiceMock.Object,
//                 _paginationServiceMock.Object,
//                 _responseMapperMock.Object);

//             var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
//             {
//                 new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
//                 new Claim(ClaimTypes.Role, "Lister")
//             }, "mock"));

//             _controller.ControllerContext = new ControllerContext
//             {
//                 HttpContext = new DefaultHttpContext { User = user }
//             };
//         }

//         [Test]
//         public async Task Create_ReturnsOkResult_WithCreatedProfile()
//         {
//             var dto = new ListerProfileAddRequestDto { LicenseNumber = "LIC123", BusinessPhoneNumber = "9876543210" };
//             var created = new ListerProfile { Id = Guid.NewGuid(), LicenseNumber = "LIC123", BusinessPhoneNumber = "9876543210" };
//             var response = new ApiResponse<ListerProfile> { Data = created };

//             _listerProfileServiceMock.Setup(x => x.CreateListerProfileAsync(dto, It.IsAny<Guid>())).ReturnsAsync(created);
//             _responseMapperMock.Setup(x => x.MapToOkResponse("Lister profile created successfully", created)).Returns(response);

//             var result = await _controller.Create(dto);

//             Assert.That(result, Is.InstanceOf<OkObjectResult>());
//             Assert.That(((OkObjectResult)result).Value, Is.EqualTo(response));
//         }

//         [Test]
//         public async Task GetAll_ReturnsProfiles()
//         {
//             var profiles = new List<ListerProfile>
//             {
//                 new ListerProfile { Id = Guid.NewGuid(), LicenseNumber = "LIC1" },
//                 new ListerProfile { Id = Guid.NewGuid(), LicenseNumber = "LIC2" }
//             };
//             var paginated = new List<ListerProfile> { profiles[0] };
//             var meta = new PaginationInfoDto();
//             var response = new ApiResponse<List<ListerProfile>>();

//             _listerProfileServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(profiles);
//             _paginationServiceMock.Setup(p => p.ApplyPagination(profiles, 1, 10)).Returns((paginated, meta));
//             _responseMapperMock.Setup(m => m.MapToOkResponse("All Lister profiles fetched successfully", paginated, meta)).Returns(response);

//             var result = await _controller.GetAll(1, 10);

//             Assert.That(result, Is.InstanceOf<OkObjectResult>());
//         }

//         [Test]
//         public async Task GetById_ReturnsOk_WhenIdIsValid()
//         {
//             var id = Guid.NewGuid();
//             var dto = new ListerProfile { Id = id, LicenseNumber = "LIC123" };
//             var response = new ApiResponse<ListerProfile>();

//             _listerProfileServiceMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(dto);
//             _responseMapperMock.Setup(x => x.MapToOkResponse("Lister Profile fetched by ID", dto)).Returns(response);

//             var result = await _controller.GetById(id);

//             Assert.That(result, Is.InstanceOf<OkObjectResult>());
//         }

//         [Test]
//         public async Task GetByListerId_ReturnsOk_WhenFound()
//         {
//             var listerId = Guid.NewGuid();
//             var dto = new ListerProfile { Id = Guid.NewGuid(), LicenseNumber = "LIC456" };
//             var response = new ApiResponse<ListerProfile>();

//             _listerProfileServiceMock.Setup(x => x.GetListerProfileByListerIdAsync(listerId)).ReturnsAsync(dto);
//             _responseMapperMock.Setup(x => x.MapToOkResponse("Lister profile fetched successfully", dto)).Returns(response);

//             var result = await _controller.GetByListerId(listerId);

//             Assert.That(result, Is.InstanceOf<OkObjectResult>());
//         }

//         [Test]
//         public void GetByListerId_ThrowsNotFound_WhenNullReturned()
//         {
//             var listerId = Guid.NewGuid();
//             _listerProfileServiceMock.Setup(x => x.GetListerProfileByListerIdAsync(listerId)).ReturnsAsync((ListerProfile?)null);

//             Assert.ThrowsAsync<NotFoundException>(() => _controller.GetByListerId(listerId));
//         }

//         [Test]
//         public async Task Update_ReturnsOk_WhenUpdated()
//         {
//             var profileId = Guid.NewGuid();
//             var dto = new ListerProfileAddRequestDto { LicenseNumber = "LIC789", BusinessPhoneNumber = "1234567890" };
//             var updated = new ListerProfile { Id = profileId, LicenseNumber = "LIC789" };
//             var response = new ApiResponse<ListerProfile>();

//             _listerProfileServiceMock.Setup(x => x.UpdateListerProfileAsync(profileId, dto, It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(updated);
//             _responseMapperMock.Setup(x => x.MapToOkResponse("Lister profile updated successfully", updated)).Returns(response);

//             var result = await _controller.Update(profileId, dto);

//             Assert.That(result, Is.InstanceOf<OkObjectResult>());
//         }
//     }
// }