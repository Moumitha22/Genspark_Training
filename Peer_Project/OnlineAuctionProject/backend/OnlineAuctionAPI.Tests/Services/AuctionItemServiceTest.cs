using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Exceptions;
using OnlineAuctionAPI.Hubs;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Mapping;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;
using OnlineAuctionAPI.Repositories;
using OnlineAuctionAPI.Services;

namespace OnlineAuctionAPI.Tests.Services
{
    [TestFixture]
    public class AuctionItemServiceTest
    {
        private AuctionContext _context;
        private IAuctionItemRepository _auctionRepo;
        private IUserRepository _userRepo;
        private IBidItemRepository _bidRepo;
        private IMapper _mapper;
        private IHttpContextAccessor _httpContextAccessor;

        private AuctionItemService _service;
        private Mock<IUserService> _mockUserService;
        private Mock<IEmailService> _mockEmailService;
        private Mock<IVirtualWalletRepository> _mockVirtualWalletRepo;
        private Mock<IEAgreementRepository> _mockEAgreementRepo;

        [SetUp]
        public void Setup()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            _mockUserService = new Mock<IUserService>();

            var options = new DbContextOptionsBuilder<AuctionContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new AuctionContext(options);

            _auctionRepo = new AuctionRepository(_context);
            _userRepo = new UserRepository(_context);
            _bidRepo = new BidItemRepository(_context);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AuctionItemProfile>();
            });
            _mapper = config.CreateMapper();

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
            _httpContextAccessor = mockHttpContextAccessor.Object;

            var mockHubClients = new Mock<IHubClients>();
            mockHubClients.Setup(clients => clients.All).Returns(new Mock<IClientProxy>().Object);

            var mockHubContext = new Mock<IHubContext<AuctionHub>>();
            mockHubContext.Setup(x => x.Clients).Returns(mockHubClients.Object);

            _mockEmailService = new Mock<IEmailService>();
            _mockVirtualWalletRepo = new Mock<IVirtualWalletRepository>();
            _mockEAgreementRepo = new Mock<IEAgreementRepository>();

            _service = new AuctionItemService(
                _auctionRepo,
                _userRepo,
                _bidRepo,
                _mapper,
                _httpContextAccessor,
                _context,
                _mockUserService.Object,
                _mockEmailService.Object,
                _mockVirtualWalletRepo.Object,
                _mockEAgreementRepo.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddAuctionItemAync_Valid_ReturnsResponse()
        {
            var seller = new User
            {
                Id = Guid.NewGuid(),
                Role = UserRole.Seller,
                Username = "seller",
                Password = "password",
            };
            _context.Users.Add(seller);
            await _context.SaveChangesAsync();

            var claims = new List<Claim> { new Claim("UserId", seller.Id.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            _service = new AuctionItemService(
                _auctionRepo,
                _userRepo,
                _bidRepo,
                _mapper,
                mockHttpContextAccessor.Object,
                _context,
                _mockUserService.Object,
                _mockEmailService.Object,
                _mockVirtualWalletRepo.Object,
                _mockEAgreementRepo.Object
            );
            var dto = new AuctionItemAddDto
            {
                Name = "Test Auction",
                SellerId = seller.Id,
                StartingPrice = 100,
                ReservePrice = 150,
                StartTime = DateTime.UtcNow.AddMinutes(5),
                EndTime = DateTime.UtcNow.AddDays(1),
            };

            var result = await _service.AddAuctionItemAsync(dto);

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo(dto.Name));
        }

        [Test]
        public void AddAuctionItemAync_InvalidSeller_ThrowsNotFound()
        {
            var invalidSellerId = Guid.NewGuid();

            var claims = new List<Claim> { new Claim("UserId", invalidSellerId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            _service = new AuctionItemService(
                _auctionRepo,
                _userRepo,
                _bidRepo,
                _mapper,
                mockHttpContextAccessor.Object,
                _context,
                _mockUserService.Object,
                _mockEmailService.Object,
                _mockVirtualWalletRepo.Object,
                _mockEAgreementRepo.Object
            );

            var dto = new AuctionItemAddDto
            {
                Name = "Test Auction",
                SellerId = invalidSellerId,
                StartingPrice = 100,
                ReservePrice = 150,
                StartTime = DateTime.UtcNow.AddMinutes(5),
                EndTime = DateTime.UtcNow.AddDays(1),
            };

            var ex = Assert.ThrowsAsync<RepositoryOperationException>(async () =>
                await _service.AddAuctionItemAsync(dto)
            );
            Assert.That(ex.InnerException, Is.TypeOf<NotFoundException>());
            Assert.That(ex.InnerException.Message, Does.Contain("not found"));
        }

        [Test]
        public async Task DeleteAuctionItemAsync_Valid_ReturnsTrue()
        {
            var seller = new User
            {
                Id = Guid.NewGuid(),
                Role = UserRole.Seller,
                Username = "seller",
                Password = "password",
            };
            var auction = new AuctionItem
            {
                Id = Guid.NewGuid(),
                Name = "Auction",
                SellerId = seller.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            _context.Users.Add(seller);
            _context.AuctionItems.Add(auction);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAuctionItemAsync(auction.Id);
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteAuctionItemAsync_NotFound_ThrowsNotFound()
        {
            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
                await _service.DeleteAuctionItemAsync(Guid.NewGuid())
            );
            Assert.That(ex.Message, Does.Contain("There is no data present"));
        }

        [Test]
        public async Task GetAllAuctionItemAsync_Valid_ReturnsList()
        {
            var seller = new User
            {
                Id = Guid.NewGuid(),
                Role = UserRole.Seller,
                Username = "seller",
                Password = "password",
            };
            var auction = new AuctionItem
            {
                Id = Guid.NewGuid(),
                Name = "Auction",
                SellerId = seller.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            _context.Users.Add(seller);
            _context.AuctionItems.Add(auction);
            await _context.SaveChangesAsync();

            var result = await _service.GetAllAuctionItemAsync();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }

        [Test]
        public void GetAllAuctionItemAsync_Empty_ThrowsNotFound()
        {
            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
                await _service.GetAllAuctionItemAsync()
            );
            Assert.That(ex.Message, Does.Contain("no data present"));
        }

        [Test]
        public async Task GetAuctionItemByIdAsync_Valid_ReturnsItem()
        {
            var seller = new User
            {
                Id = Guid.NewGuid(),
                Role = UserRole.Seller,
                Username = "seller",
                Password = "password",
            };
            var auction = new AuctionItem
            {
                Id = Guid.NewGuid(),
                Name = "Auction",
                SellerId = seller.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            _context.Users.Add(seller);
            _context.AuctionItems.Add(auction);
            await _context.SaveChangesAsync();

            var result = await _service.GetAuctionItemByIdAsync(auction.Id);
            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo(auction.Name));
        }

        [Test]
        public void GetAuctionItemByIdAsync_NotFound_ThrowsNotFound()
        {
            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
                await _service.GetAuctionItemByIdAsync(Guid.NewGuid())
            );
            Assert.That(ex.Message, Does.Contain("not found"));
        }

        [Test]
        public async Task UpdateAuctionItemAsync_Valid_ReturnsUpdated()
        {
            var seller = new User
            {
                Id = Guid.NewGuid(),
                Role = UserRole.Seller,
                Username = "seller",
                Password = "password",
            };
            var auction = new AuctionItem
            {
                Id = Guid.NewGuid(),
                Name = "Auction",
                SellerId = seller.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            _context.Users.Add(seller);
            _context.AuctionItems.Add(auction);
            await _context.SaveChangesAsync();

            var updateDto = new AuctionItemAddDto
            {
                Name = "Updated Auction",
                SellerId = seller.Id,
            };

            var result = await _service.UpdateAuctionItemAsync(auction.Id, updateDto);
            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo("Updated Auction"));
        }

        [Test]
        public void UpdateAuctionItemAsync_NotFound_ThrowsNotFound()
        {
            var updateDto = new AuctionItemAddDto { Name = "Updated" };
            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
                await _service.UpdateAuctionItemAsync(Guid.NewGuid(), updateDto)
            );
            Assert.That(ex.Message, Does.Contain("Auction item with ID"));
        }

        [Test]
        public async Task GetPagedAuctionItemsAsync_Valid_ReturnsPaged()
        {
            var seller = new User
            {
                Id = Guid.NewGuid(),
                Role = UserRole.Seller,
                Username = "seller",
                Password = "password",
            };
            _context.Users.Add(seller);
            for (int i = 0; i < 5; i++)
            {
                _context.AuctionItems.Add(
                    new AuctionItem
                    {
                        Id = Guid.NewGuid(),
                        Name = $"Auction {i}",
                        SellerId = seller.Id,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    }
                );
            }
            await _context.SaveChangesAsync();

            var pagination = new PaginationDto { Page = 1, PageSize = 2 };
            var result = await _service.GetPagedAuctionItemsAsync(pagination);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Any());
            Assert.That(result.Data.Count(), Is.EqualTo(2));
        }

        [Test]
        public void UpdateWinningId_NullDto_ThrowsArgumentException()
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _service.UpdateWinningId(null)
            );
            Assert.That(ex.Message, Does.Contain("Invalid winning ID update data"));
        }

        [Test]
        public async Task UpdateWinningId_Valid_ReturnsResponse()
        {
            var seller = new User
            {
                Id = Guid.NewGuid(),
                Role = UserRole.Seller,
                Username = "seller",
                Password = "password",
            };
            var bidder = new User
            {
                Id = Guid.NewGuid(),
                Role = UserRole.Bidder,
                Username = "bidder",
                Password = "password",
                Email = "bidder@example.com",
            };
            var auction = new AuctionItem
            {
                Id = Guid.NewGuid(),
                Name = "Auction",
                SellerId = seller.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddMinutes(10),
            };
            var bid = new BidItem
            {
                Id = Guid.NewGuid(),
                AuctionItemId = auction.Id,
                BidderId = bidder.Id,
                Amount = 200,
                BidTime = DateTime.UtcNow,
            };
            var wallet = new VirtualWallet
            {
                Id = Guid.NewGuid(),
                UserId = bidder.Id,
                Balance = 1000,
                UpdatedAt = DateTime.UtcNow,
            };

            _context.Users.Add(seller);
            _context.Users.Add(bidder);
            _context.AuctionItems.Add(auction);
            _context.BidItems.Add(bid);
            await _context.SaveChangesAsync();

            _mockVirtualWalletRepo.Setup(r => r.GetByUserIdAsync(bidder.Id)).ReturnsAsync(wallet);
            _mockVirtualWalletRepo
                .Setup(r => r.Update(wallet.Id, It.IsAny<VirtualWallet>()))
                .ReturnsAsync(wallet);
            _mockVirtualWalletRepo
                .Setup(r => r.AddHistoryAsync(It.IsAny<VirtualWalletHistory>()))
                .Returns(Task.CompletedTask);

            _mockEAgreementRepo
                .Setup(r => r.Add(It.IsAny<EAgreement>()))
                .ReturnsAsync((EAgreement e) => e);

            _mockUserService.Setup(s => s.GetUserByIdAsync(bidder.Id)).ReturnsAsync(bidder);

            _mockEmailService
                .Setup(e =>
                    e.SendEmailAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()
                    )
                )
                .Returns(Task.CompletedTask);

            var updateDto = new WinningIdUpdateDto
            {
                AuctionItemId = auction.Id,
                WinningId = bid.Id,
            };

            var result = await _service.UpdateWinningId(updateDto);

            Assert.IsNotNull(result);
            Assert.That(result.AuctionItemId, Is.EqualTo(auction.Id));
            Assert.That(result.WinnerId, Is.EqualTo(bid.BidderId));
        }

        [Test]
        public void UpdateWinningId_InvalidUser_ThrowsNotFoundException()
        {
            var updateDto = new WinningIdUpdateDto
            {
                AuctionItemId = Guid.NewGuid(),
                WinningId = Guid.NewGuid(),
            };

            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
                await _service.UpdateWinningId(updateDto)
            );
            Assert.That(ex.Message, Does.Contain("Bid not found for the provided WinningId."));
        }
    }
}
