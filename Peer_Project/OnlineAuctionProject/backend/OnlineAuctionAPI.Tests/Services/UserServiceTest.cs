using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Exceptions;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Mapping;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;
using OnlineAuctionAPI.Repositories;
using OnlineAuctionAPI.Services;

namespace OnlineAuctionAPI.Tests.Services
{
    public class UserServiceTests
    {
        private IUserService _userService;
        private IUserRepository _userRepository;
        private IMapper _mapper;
        private Mock<IPasswordService> _passwordServiceMock;
        private AuctionContext _dbContext;

        private Mock<IEmailService> _emailServiceMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AuctionContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new AuctionContext(options);
            _userRepository = new UserRepository(_dbContext);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>());
            _mapper = config.CreateMapper();

            _passwordServiceMock = new Mock<IPasswordService>();
            _passwordServiceMock
                .Setup(p => p.HashPassword(It.IsAny<string>()))
                .Returns("hashedPwd");

            _emailServiceMock = new Mock<IEmailService>();

            _userService = new UserService(
                _userRepository,
                _mapper,
                _passwordServiceMock.Object,
                _dbContext,
                _emailServiceMock.Object
            );
        }

        [Test]
        public async Task CreateUserAsync_Should_Create_User_Successfully()
        {
            var dto = new UserRegisterRequestDto
            {
                Email = "test@example.com",
                Password = "123456",
                Role = "Seller",
            };

            var result = await _userService.CreateUserAsync(dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(dto.Email, result.Email);
        }

        [Test]
        public async Task CreateUserAsync_Should_Throw_AlreadyExistsException_When_Email_Exists()
        {
            var dto = new UserRegisterRequestDto
            {
                Email = "test@example.com",
                Password = "123456",
                Role = "Seller",
            };

            await _userService.CreateUserAsync(dto);

            var ex = Assert.ThrowsAsync<AlreadyExistsException>(() =>
                _userService.CreateUserAsync(dto)
            );
            Assert.That(ex.Message, Is.EqualTo("Some user already exists with this email."));
        }

        [Test]
        public void GetUserByEmailAsync_Should_Throw_When_Email_Null()
        {
            var ex = Assert.ThrowsAsync<NullValueException>(() =>
                _userService.GetUserByEmailAsync(null)
            );
            Assert.That(ex.Message, Does.Contain("Email can't be null"));
        }

        [Test]
        public async Task GetUserByEmailAsync_Should_Return_User_When_Found()
        {
            var user = new User
            {
                Email = "user@example.com",
                Username = "user",
                Password = "123456",
            };
            await _userRepository.Add(user);

            var result = await _userService.GetUserByEmailAsync("user@example.com");

            Assert.IsNotNull(result);
            Assert.AreEqual(user.Email, result.Email);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }
    }
}
