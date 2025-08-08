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

namespace OnlineAuctionAPI.Tests.Controllers
{
    public class AuctionDeleteRequestControllerTests
    {
        private Mock<IAuctionDeleteRequestService> _mockService;
        private Mock<ILogger<AuctionDeleteRequestController>> _mockLogger;
        private AuctionDeleteRequestController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IAuctionDeleteRequestService>();
            _mockLogger = new Mock<ILogger<AuctionDeleteRequestController>>();
            _controller = new AuctionDeleteRequestController(
                _mockService.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task SubmitAuctionDeleteRequest_ReturnsOk()
        {
            var dto = new AuctionDeleteRequestDto
            {
                AuctionItemId = Guid.NewGuid(),
                Reason = "Test",
                UserId = Guid.NewGuid(),
            };
            _mockService
                .Setup(s => s.SubmitAuctionDeleteRequestAsync(dto))
                .Returns(Task.CompletedTask);

            var result = await _controller.SubmitAuctionDeleteRequest(dto);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task ApproveAuctionDeleteRequest_ReturnsOk()
        {
            var auctionId = Guid.NewGuid();
            _mockService
                .Setup(s => s.ApproveAuctionDeleteRequestAsync(auctionId))
                .Returns(Task.CompletedTask);

            var result = await _controller.ApproveAuctionDeleteRequest(auctionId);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task RejectAuctionDeleteRequest_ReturnsOk()
        {
            var dto = new AuctionDeleteRequestRejectDto
            {
                AuctionItemId = Guid.NewGuid(),
                Remarks = "Rejected",
            };
            _mockService
                .Setup(s => s.RejectAuctionDeleteRequestAsync(dto))
                .Returns(Task.CompletedTask);

            var result = await _controller.RejectAuctionDeleteRequest(dto);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetAuctionDeleteRequestByAuctionId_ReturnsOk_WhenFound()
        {
            var auctionId = Guid.NewGuid();
            var request = new AuctionDeleteRequest
            {
                Id = Guid.NewGuid(),
                AuctionItemId = auctionId,
                Reason = "Test",
                UserId = Guid.NewGuid(),
            };
            _mockService
                .Setup(s => s.GetAuctionDeleteRequestByAuctionIdAsync(auctionId))
                .ReturnsAsync(request);

            var result = await _controller.GetAuctionDeleteRequestByAuctionId(auctionId);

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test]
        public async Task GetAuctionDeleteRequestByAuctionId_ReturnsNotFound_WhenNotFound()
        {
            var auctionId = Guid.NewGuid();
            _mockService
                .Setup(s => s.GetAuctionDeleteRequestByAuctionIdAsync(auctionId))
                .ReturnsAsync((AuctionDeleteRequest)null);

            var result = await _controller.GetAuctionDeleteRequestByAuctionId(auctionId);

            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
        }
    }
}
