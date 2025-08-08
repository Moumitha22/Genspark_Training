using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Services;

namespace OnlineAuctionAPI.Tests.Service
{
    public class EAgreementServiceTests
    {
        private Mock<IEAgreementRepository> _mockEAgreementRepo;
        private Mock<IBidItemRepository> _mockBidItemRepo;
        private EAgreementService _service;

        [SetUp]
        public void Setup()
        {
            _mockEAgreementRepo = new Mock<IEAgreementRepository>();
            _mockBidItemRepo = new Mock<IBidItemRepository>();
            _service = new EAgreementService(_mockEAgreementRepo.Object, _mockBidItemRepo.Object);
        }

        [Test]
        public async Task GetByUserIdAsync_ReturnsAgreementsForUser()
        {
            var userId = Guid.NewGuid();
            var agreements = new List<EAgreement>
            {
                new EAgreement
                {
                    Id = Guid.NewGuid(),
                    AuctionItemId = Guid.NewGuid(),
                    BiddingId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    Bidding = new BidItem { BidderId = userId },
                },
                new EAgreement
                {
                    Id = Guid.NewGuid(),
                    AuctionItemId = Guid.NewGuid(),
                    BiddingId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    Bidding = new BidItem { BidderId = Guid.NewGuid() },
                },
            };

            _mockEAgreementRepo.Setup(r => r.GetAllWithBidItemsAsync()).ReturnsAsync(agreements);

            var result = await _service.GetByUserIdAsync(userId.ToString());

            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task GetByIdAsync_ReturnsAgreement()
        {
            var id = Guid.NewGuid();
            var agreement = new EAgreement { Id = id };
            _mockEAgreementRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(agreement);

            var result = await _service.GetByIdAsync(id);

            Assert.AreEqual(agreement, result);
        }

        [Test]
        public async Task GetByBiddingIdAsync_ReturnsAgreements()
        {
            var biddingId = Guid.NewGuid();
            var agreements = new List<EAgreement> { new EAgreement { BiddingId = biddingId } };
            _mockEAgreementRepo
                .Setup(r => r.GetByBiddingIdAsync(biddingId))
                .ReturnsAsync(agreements);

            var result = await _service.GetByBiddingIdAsync(biddingId);

            Assert.AreEqual(agreements, result);
        }

        [Test]
        public async Task GetByUserIdAsync_ReturnsZeroAgreementsForUser_WhenNoMatch()
        {
            var userId = Guid.NewGuid();
            var agreements = new List<EAgreement>
            {
                new EAgreement
                {
                    Id = Guid.NewGuid(),
                    AuctionItemId = Guid.NewGuid(),
                    BiddingId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    Bidding = new BidItem { BidderId = Guid.NewGuid() },
                },
                new EAgreement
                {
                    Id = Guid.NewGuid(),
                    AuctionItemId = Guid.NewGuid(),
                    BiddingId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    Bidding = new BidItem { BidderId = Guid.NewGuid() },
                },
            };

            _mockEAgreementRepo.Setup(r => r.GetAllWithBidItemsAsync()).ReturnsAsync(agreements);

            var result = await _service.GetByUserIdAsync(userId.ToString());

            Assert.AreEqual(0, result.Count());
        }
    }
}
