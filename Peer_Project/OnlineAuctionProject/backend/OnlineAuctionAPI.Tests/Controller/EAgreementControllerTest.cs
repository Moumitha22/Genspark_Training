using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OnlineAuctionAPI.Controllers;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Services;

namespace OnlineAuctionAPI.Tests.Controllers
{
    public class EAgreementControllerTests
    {
        private Mock<IEAgreementService> _mockService;
        private Mock<IEmailService> _mockEmailService;
        private Mock<ILogger<EAgreementController>> _mockLogger;
        private EAgreementController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IEAgreementService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockLogger = new Mock<ILogger<EAgreementController>>();
            _controller = new EAgreementController(
                _mockService.Object,
                _mockEmailService.Object,
                _mockLogger.Object
            );
        }

        private void SetUser(Guid userId)
        {
            var claims = new List<Claim> { new Claim("sub", userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = principal },
            };
        }

        [Test]
        public async Task GetMyAgreements_ReturnsOkWithAgreements()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            var agreements = new List<object> { new { Id = Guid.NewGuid() } };
            _mockService.Setup(s => s.GetByUserIdAsync(userId.ToString())).ReturnsAsync(agreements);

            var result = await _controller.GetMyAgreements();

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetMyAgreements_InvalidUserId_ReturnsBadRequest()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext(),
            };

            var result = await _controller.GetMyAgreements();

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task DownloadAgreementFile_ReturnsFile()
        {
            var id = Guid.NewGuid();
            var agreement = new EAgreement { Id = id, File = new byte[] { 1, 2, 3 } };
            _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(agreement);

            var result = await _controller.DownloadAgreementFile(id);

            Assert.IsInstanceOf<FileContentResult>(result);
        }

        [Test]
        public async Task DownloadAgreementFile_NotFound_ReturnsNotFound()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((EAgreement)null);

            var result = await _controller.DownloadAgreementFile(id);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task GetAgreementsByBiddingId_ReturnsOkWithAgreements()
        {
            var biddingId = Guid.NewGuid();
            var agreements = new List<EAgreement> { new EAgreement { Id = Guid.NewGuid() } };
            _mockService.Setup(s => s.GetByBiddingIdAsync(biddingId)).ReturnsAsync(agreements);

            var result = await _controller.GetAgreementsByBiddingId(biddingId);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetAgreementsByBiddingId_NotFound_ReturnsNotFound()
        {
            var biddingId = Guid.NewGuid();
            _mockService
                .Setup(s => s.GetByBiddingIdAsync(biddingId))
                .ReturnsAsync(new List<EAgreement>());

            var result = await _controller.GetAgreementsByBiddingId(biddingId);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
    }
}
