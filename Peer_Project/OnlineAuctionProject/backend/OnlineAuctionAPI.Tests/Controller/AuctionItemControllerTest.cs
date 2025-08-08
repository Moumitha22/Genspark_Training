using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Controllers;
using OnlineAuctionAPI.Hubs;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Tests.Controllers
{
    [TestFixture]
    public class AuctionItemControllerTest
    {
        private Mock<IAuctionItemService> _mockService;
        private Mock<AuctionContext> _mockContext;
        private AuctionItemController _controller;
        private Mock<ILogger<AuctionItemController>> _mockLogger;
        private Mock<IHubContext<AuctionHub>> _mockHubContext;
        private Mock<IHubClients> _mockClients;
        private Mock<IClientProxy> _mockClientProxy;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IAuctionItemService>();
            var options = new DbContextOptionsBuilder<AuctionContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _mockContext = new Mock<AuctionContext>(options);
            _mockLogger = new Mock<ILogger<AuctionItemController>>();
            _mockHubContext = new Mock<IHubContext<AuctionHub>>();
            _mockClients = new Mock<IHubClients>();
            _mockClientProxy = new Mock<IClientProxy>();

            _mockHubContext.Setup(x => x.Clients).Returns(_mockClients.Object);
            _mockClients.Setup(x => x.All).Returns(_mockClientProxy.Object);
            _mockClientProxy
                .Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);

            _controller = new AuctionItemController(
                _mockService.Object,
                _mockContext.Object,
                _mockLogger.Object,
                _mockHubContext.Object
            );
        }

        [Test]
        public async Task GetAuctionById_ReturnsOkWithData()
        {
            var auctionId = Guid.NewGuid();
            var responseDto = new AuctionItemResponseDto { Id = auctionId };
            _mockService.Setup(s => s.GetAuctionItemByIdAsync(auctionId)).ReturnsAsync(responseDto);

            var actionResult = await _controller.GetAuctionById(auctionId);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var apiResponse = okResult.Value as ApiResponse<AuctionItemResponseDto>;
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(auctionId, apiResponse.Data.Id);
        }

        [Test]
        public async Task GetAuctionById_NotFound_ReturnsNotFound()
        {
            var auctionId = Guid.NewGuid();
            _mockService
                .Setup(s => s.GetAuctionItemByIdAsync(auctionId))
                .ReturnsAsync((AuctionItemResponseDto)null);

            var actionResult = await _controller.GetAuctionById(auctionId);
            var notFoundResult = actionResult.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);

            var apiResponse = notFoundResult.Value as ApiResponse<string>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Auction item not found", apiResponse.Message);
        }

        [Test]
        public async Task GetAuctionById_Exception_ThrowsException()
        {
            var auctionId = Guid.NewGuid();
            _mockService
                .Setup(s => s.GetAuctionItemByIdAsync(auctionId))
                .ThrowsAsync(new Exception("DB error"));

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _controller.GetAuctionById(auctionId)
            );
            Assert.That(ex.Message, Does.Contain("DB error"));
        }

        [Test]
        public async Task GetAllAuctions_ReturnsOkWithList()
        {
            var list = new List<AuctionItemResponseDto> { new AuctionItemResponseDto() };
            _mockService.Setup(s => s.GetAllAuctionItemAsync()).ReturnsAsync(list);

            var actionResult = await _controller.GetAllAuctions();
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var apiResponse = okResult.Value as ApiResponse<IEnumerable<AuctionItemResponseDto>>;
            Assert.IsTrue(apiResponse.Success);
            Assert.IsNotEmpty(apiResponse.Data);
        }

        [Test]
        public async Task GetAllAuctions_Exception_ThrowsException()
        {
            _mockService
                .Setup(s => s.GetAllAuctionItemAsync())
                .ThrowsAsync(new Exception("DB error"));

            var ex = Assert.ThrowsAsync<Exception>(async () => await _controller.GetAllAuctions());
            Assert.That(ex.Message, Does.Contain("DB error"));
        }

        [Test]
        public async Task AddAuctionItem_ReturnsCreatedAtAction()
        {
            var dto = new AuctionItemAddDto();
            var responseDto = new AuctionItemResponseDto { Id = Guid.NewGuid() };
            _mockService
                .Setup(s => s.AddAuctionItemAsync(It.IsAny<AuctionItemAddDto>()))
                .ReturnsAsync(responseDto);
            var actionResult = await _controller.AddAuctionItem(dto);
            var createdResult = actionResult.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            var apiResponse = createdResult.Value as ApiResponse<AuctionItemResponseDto>;
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(responseDto.Id, apiResponse.Data.Id);
        }

        [Test]
        public async Task AddAuctionItem_Exception_ThrowsException()
        {
            var dto = new AuctionItemAddDto();
            _mockService
                .Setup(s => s.AddAuctionItemAsync(dto))
                .ThrowsAsync(new Exception("Invalid data"));

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _controller.AddAuctionItem(dto)
            );
            Assert.That(ex.Message, Does.Contain("Invalid data"));
        }

        [Test]
        public async Task DeleteAuctionItem_ReturnsOkWithTrue()
        {
            var auctionId = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteAuctionItemAsync(auctionId)).ReturnsAsync(true);

            var result = await _controller.DeleteAuctionItem(auctionId) as OkObjectResult;
            Assert.IsNotNull(result);
            var apiResponse = result.Value as ApiResponse<bool>;
            Assert.IsTrue(apiResponse.Success);
            Assert.IsTrue(apiResponse.Data);
        }

        [Test]
        public async Task DeleteAuctionItem_Exception_ThrowsException()
        {
            var auctionId = Guid.NewGuid();
            _mockService
                .Setup(s => s.DeleteAuctionItemAsync(auctionId))
                .ThrowsAsync(new Exception("Delete failed"));

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _controller.DeleteAuctionItem(auctionId)
            );
            Assert.That(ex.Message, Does.Contain("Delete failed"));
        }

        [Test]
        public async Task UpdateWinningId_ReturnsOkWithWinnerIdResponse()
        {
            var dto = new WinningIdUpdateDto();
            var responseDto = new WinnerIdResponseDto();
            _mockService.Setup(s => s.UpdateWinningId(dto)).ReturnsAsync(responseDto);

            var actionResult = await _controller.UpdateWinningId(dto);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var apiResponse = okResult.Value as ApiResponse<WinnerIdResponseDto>;
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(responseDto, apiResponse.Data);
        }

        [Test]
        public async Task UpdateWinningId_Exception_ReturnsBadRequest()
        {
            var dto = new WinningIdUpdateDto();
            _mockService
                .Setup(s => s.UpdateWinningId(dto))
                .ThrowsAsync(new Exception("Update failed"));

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _controller.UpdateWinningId(dto)
            );
            Assert.That(ex.Message, Does.Contain("Update failed"));
        }
    }
}
