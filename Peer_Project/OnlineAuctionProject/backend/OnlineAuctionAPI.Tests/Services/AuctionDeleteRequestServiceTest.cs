using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;
using OnlineAuctionAPI.Services;

namespace OnlineAuctionAPI.Tests.Service
{
    public class AuctionDeleteRequestServiceTests
    {
        private Mock<IAuctionDeleteRequestRepository> _mockRepo;
        private AuctionDeleteRequestService _service;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IAuctionDeleteRequestRepository>();
            _service = new AuctionDeleteRequestService(_mockRepo.Object);
        }

        [Test]
        public async Task SubmitAuctionDeleteRequestAsync_CallsRepository()
        {
            var dto = new AuctionDeleteRequestDto();
            _mockRepo.Setup(r => r.AuctionDeleteRequestAsync(dto)).Returns(Task.CompletedTask);

            await _service.SubmitAuctionDeleteRequestAsync(dto);

            _mockRepo.Verify(r => r.AuctionDeleteRequestAsync(dto), Times.Once);
        }

        [Test]
        public void RejectAuctionDeleteRequestAsync_NullDto_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _service.RejectAuctionDeleteRequestAsync(null)
            );
        }

        [Test]
        public async Task RejectAuctionDeleteRequestAsync_ValidDto_CallsRepository()
        {
            var dto = new AuctionDeleteRequestRejectDto();
            _mockRepo
                .Setup(r => r.RejectAuctionDeleteRequestAsync(dto))
                .Returns(Task.CompletedTask);

            await _service.RejectAuctionDeleteRequestAsync(dto);

            _mockRepo.Verify(r => r.RejectAuctionDeleteRequestAsync(dto), Times.Once);
        }

        [Test]
        public async Task GetAuctionDeleteRequestByAuctionIdAsync_ReturnsRequest()
        {
            var id = Guid.NewGuid();
            var request = new AuctionDeleteRequest { Id = id };
            _mockRepo
                .Setup(r => r.GetAuctionDeleteRequestByAuctionIdAsync(id))
                .ReturnsAsync(request);

            var result = await _service.GetAuctionDeleteRequestByAuctionIdAsync(id);

            Assert.AreEqual(request, result);
        }

        [Test]
        public void ApproveAuctionDeleteRequestAsync_EmptyGuid_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _service.ApproveAuctionDeleteRequestAsync(Guid.Empty)
            );
        }

        [Test]
        public void ApproveAuctionDeleteRequestAsync_NotFound_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            _mockRepo
                .Setup(r => r.GetAuctionDeleteRequestByAuctionIdAsync(id))
                .ReturnsAsync((AuctionDeleteRequest)null);

            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _service.ApproveAuctionDeleteRequestAsync(id)
            );
        }

        [Test]
        public async Task ApproveAuctionDeleteRequestAsync_ValidRequest_UpdatesRequest()
        {
            var id = Guid.NewGuid();
            var request = new AuctionDeleteRequest { Id = id, IsApproved = false };
            _mockRepo
                .Setup(r => r.GetAuctionDeleteRequestByAuctionIdAsync(id))
                .ReturnsAsync(request);
            _mockRepo
                .Setup(r => r.GetAuctionDeleteRequestByAuctionIdAsync(id))
                .ReturnsAsync(request);

            await _service.ApproveAuctionDeleteRequestAsync(id);

            _mockRepo.Verify(
                r => r.Update(id, It.Is<AuctionDeleteRequest>(r => r.IsApproved)),
                Times.Once
            );
        }
    }
}
