using System;
using System.Collections.Generic;
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
    public class BidItemControllerTest
    {
        private Mock<IBidItemService> _mockBidService;
        private Mock<ILogger<BidItemController>> _mockLogger;
        private Mock<IHubContext<AuctionHub>> _mockHubContext;
        private BidItemController _controller;
        private Mock<IHubClients> _mockClients;
        private Mock<IClientProxy> _mockClientProxy;

        [SetUp]
        public void Setup()
        {
            _mockBidService = new Mock<IBidItemService>();
            _mockLogger = new Mock<ILogger<BidItemController>>();
            _mockHubContext = new Mock<IHubContext<AuctionHub>>();
            _mockClients = new Mock<IHubClients>();
            _mockClientProxy = new Mock<IClientProxy>();
            _mockClients.Setup(clients => clients.All).Returns(_mockClientProxy.Object);
            _mockHubContext.Setup(hub => hub.Clients).Returns(_mockClients.Object);
            _controller = new BidItemController(
                _mockBidService.Object,
                _mockLogger.Object,
                _mockHubContext.Object
            );
        }

        [Test]
        public async Task AddBidItem_ReturnsOkWithBidResponse()
        {
            var bidDto = new BidItemAddDto
            {
                AuctionItemId = Guid.NewGuid(),
                BidderId = Guid.NewGuid(),
                Amount = 100,
            };
            var responseDto = new BidItemResponseDto();
            _mockBidService.Setup(s => s.PlaceBidAsync(bidDto)).ReturnsAsync(responseDto);

            var actionResult = await _controller.AddBidItem(bidDto);
            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            var apiResponse = okResult.Value as ApiResponse<BidItemResponseDto>;
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(responseDto, apiResponse.Data);
        }

        [Test]
        public async Task AddBidItem_Exception_ThrowsException()
        {
            var bidDto = new BidItemAddDto
            {
                AuctionItemId = Guid.NewGuid(),
                BidderId = Guid.NewGuid(),
                Amount = 100,
            };
            _mockBidService
                .Setup(s => s.PlaceBidAsync(bidDto))
                .ThrowsAsync(new Exception("Bid failed"));

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _controller.AddBidItem(bidDto)
            );
            Assert.That(ex.Message, Does.Contain("Bid failed"));
        }

        [Test]
        public async Task GetBidsByAuctionId_ReturnsOkWithList()
        {
            var auctionId = Guid.NewGuid();
            var responseList = new List<BidItemResponseDto> { new BidItemResponseDto() };
            _mockBidService
                .Setup(s => s.GetBidsByAuctionIdAsync(auctionId))
                .ReturnsAsync(responseList);

            var actionResult = await _controller.GetBidsByAuctionId(auctionId);
            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            var apiResponse = okResult.Value as ApiResponse<IEnumerable<BidItemResponseDto>>;
            Assert.IsTrue(apiResponse.Success);
            Assert.IsNotEmpty(apiResponse.Data);
        }

        [Test]
        public async Task GetBidsByAuctionId_Exception_ThrowsException()
        {
            var auctionId = Guid.NewGuid();
            _mockBidService
                .Setup(s => s.GetBidsByAuctionIdAsync(auctionId))
                .ThrowsAsync(new Exception("Get bids failed"));

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _controller.GetBidsByAuctionId(auctionId)
            );
            Assert.That(ex.Message, Does.Contain("Get bids failed"));
        }

        [Test]
        public async Task GetHighestBids_ReturnsOkWithBid()
        {
            var auctionItemId = Guid.NewGuid();
            var responseDto = new BidItemResponseDto();
            _mockBidService
                .Setup(s => s.GetHighestBidAsync(auctionItemId))
                .ReturnsAsync(responseDto);

            var actionResult = await _controller.GetHighestBids(auctionItemId);
            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            var apiResponse = okResult.Value as ApiResponse<BidItemResponseDto>;
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(responseDto, apiResponse.Data);
        }

        [Test]
        public async Task GetHighestBids_Exception_ThrowsException()
        {
            var auctionItemId = Guid.NewGuid();
            _mockBidService
                .Setup(s => s.GetHighestBidAsync(auctionItemId))
                .ThrowsAsync(new Exception("Get highest bid failed"));

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _controller.GetHighestBids(auctionItemId)
            );
            Assert.That(ex.Message, Does.Contain("Get highest bid failed"));
        }

        [Test]
        public async Task DeleteBid_ReturnsOkWithTrue()
        {
            var bidId = Guid.NewGuid();
            _mockBidService.Setup(s => s.DeleteBidAsync(bidId)).ReturnsAsync(true);

            var actionResult = await _controller.DeleteBid(bidId);
            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            var apiResponse = okResult.Value as ApiResponse<bool>;
            Assert.IsTrue(apiResponse.Success);
            Assert.IsTrue(apiResponse.Data);
        }

        [Test]
        public async Task DeleteBid_Exception_ThrowsException()
        {
            var bidId = Guid.NewGuid();
            _mockBidService
                .Setup(s => s.DeleteBidAsync(bidId))
                .ThrowsAsync(new Exception("Delete failed"));

            var ex = Assert.ThrowsAsync<Exception>(async () => await _controller.DeleteBid(bidId));
            Assert.That(ex.Message, Does.Contain("Delete failed"));
        }
    }
}
