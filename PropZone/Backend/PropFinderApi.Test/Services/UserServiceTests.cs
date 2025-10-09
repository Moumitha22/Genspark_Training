using NUnit.Framework;
using Moq;
using PropFinderApi.Services;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Exceptions;
using PropFinderApi.Models.Enums;

namespace PropFinderApi.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepoMock;
        private Mock<IListerProfileRepository> _listerProfileRepoMock;
        private IUserService _userService;

        [SetUp]
        public void SetUp()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _listerProfileRepoMock = new Mock<IListerProfileRepository>();
            _userService = new UserService(_userRepoMock.Object, _listerProfileRepoMock.Object);
        }

        [Test]
        public async Task CreateUserAsync_ValidUser_ReturnsCreatedUser()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                Role = UserRole.Buyer
            };
            _userRepoMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync(user);

            var result = await _userService.CreateUserAsync(user);

            Assert.IsNotNull(result);
            Assert.That(result.Email, Is.EqualTo(user.Email));
            _userRepoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
        }

        [Test]
        public async Task GetUserByIdAsync_UserExists_ReturnsUser()
        {
            var id = Guid.NewGuid();
            var user = new User
            {
                Id = id,
                Email = "test@example.com"
            };
            _userRepoMock.Setup(r => r.Get(id)).ReturnsAsync(user);

            var result = await _userService.GetUserByIdAsync(id);

            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(id));
        }

        [Test]
        public async Task GetUserByIdAsync_UserNotFound_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();

            _userRepoMock.Setup(r => r.Get(userId))
                        .ThrowsAsync(new NotFoundException("User not found"));

            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
                await _userService.GetUserByIdAsync(userId));

            Assert.That(ex.Message, Is.EqualTo("User not found"));
        }

        [Test]
        public async Task GetUserByEmailAsync__ReturnsUser()
        {
            var id = Guid.NewGuid();
            var email = "USER@Example.com";
            var cleanedEmail = "user@example.com";
            var user = new User
            {
                Id = id,
                Email = cleanedEmail
            };

            _userRepoMock.Setup(r => r.GetByEmailAsync(cleanedEmail)).ReturnsAsync(user);

            var result = await _userService.GetUserByEmailAsync("  USER@Example.com  ");

            Assert.IsNotNull(result);
            Assert.That(result.Email, Is.EqualTo(cleanedEmail));
        }

        [Test]
        public async Task GetUserByEmailandRoleAsync_ReturnsUser()
        {
            var id = Guid.NewGuid();
            var email = "USER@Example.com";
            var cleanedEmail = "user@example.com";
            var role = UserRole.Lister;
            var user = new User
            {
                Id = id,
                Email = cleanedEmail,
                Role = role
            };

            _userRepoMock.Setup(r => r.GetByEmailandRoleAsync(cleanedEmail, role))
                        .ReturnsAsync(user);

            var result = await _userService.GetUserByEmailandRoleAsync("  USER@Example.com  ", UserRole.Lister);

            Assert.IsNotNull(result);
            Assert.That(result.Email, Is.EqualTo(cleanedEmail));
            Assert.That(result.Role, Is.EqualTo(UserRole.Lister));
        }

        [Test]
        public async Task GetAllUsersAsync_ReturnsUserList()
        {
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Name="user2", Email = "user1@example.com", Role = UserRole.Buyer },
                new User { Id = Guid.NewGuid(), Name="user2", Email = "user2@example.com", Role = UserRole.Lister }
            };
            _userRepoMock.Setup(r => r.GetAll()).ReturnsAsync(users);

            var result = await _userService.GetAllUsersAsync();

            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task UpdateUserAsync_AsAdmin_UpdatesSuccessfully()
        {
            var id = Guid.NewGuid();
            var existingUser = new User { Id = id, Name = "Old", PhoneNumber = "111", Role = UserRole.Buyer };
            var dto = new UserUpdateRequestDto { Name = "New", PhoneNumber = "222" };
            var requesterId = Guid.NewGuid();

            _userRepoMock.Setup(r => r.Get(id)).ReturnsAsync(existingUser);
            _userRepoMock.Setup(r => r.Update(id, It.IsAny<User>())).ReturnsAsync((Guid _, User u) => u);

            var result = await _userService.UpdateUserAsync(id, dto, requesterId, "Admin");

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo("New"));
            Assert.That(result.PhoneNumber, Is.EqualTo("222"));
        }

        [Test]
        public void UpdateUserAsync_NonAdminWrongUser_ThrowsUnauthorized()
        {
            var requesterUserId = Guid.NewGuid();
            var targetUpdateUserId = Guid.NewGuid();

            var dto = new UserUpdateRequestDto();

            var ex = Assert.ThrowsAsync<UnauthorizedException>(async () =>
                await _userService.UpdateUserAsync(targetUpdateUserId, dto, requesterUserId, "Buyer"));

            Assert.That(ex.Message, Is.EqualTo("You are not authorized to update this data."));
        }

        [Test]
        public async Task UpdateUserAsync_UserNotFound_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            var requesterId = userId;
            var dto = new UserUpdateRequestDto { Name = "Test" };

            _userRepoMock.Setup(r => r.Get(userId))
                        .ThrowsAsync(new NotFoundException("User not found"));

            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
                await _userService.UpdateUserAsync(userId, dto, requesterId, "Buyer"));

            Assert.That(ex.Message, Is.EqualTo("User not found"));

            _userRepoMock.Verify(r => r.Update(It.IsAny<Guid>(), It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task UpdateUserStatusAsync_DisableLister_UpdatesUserAndProfile()
        {
            var userId = Guid.NewGuid();
            var profileId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Role = UserRole.Lister,
                IsDeleted = false
            };
            var profile = new ListerProfile
            {
                Id = profileId,
                UserId = userId,
                IsDeleted = false
            };

            _userRepoMock.Setup(r => r.Get(userId)).ReturnsAsync(user);
            _listerProfileRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(profile);
            _listerProfileRepoMock.Setup(r => r.Update(profileId, It.IsAny<ListerProfile>())).ReturnsAsync(profile);
            _userRepoMock.Setup(r => r.Update(userId, It.IsAny<User>())).ReturnsAsync(user);

            await _userService.UpdateUserStatusAsync(userId, true);

            Assert.IsTrue(user.IsDeleted);
            Assert.IsTrue(profile.IsDeleted);
            _listerProfileRepoMock.Verify(r => r.Update(profileId, It.Is<ListerProfile>(p => p.IsDeleted)), Times.Once);
            _userRepoMock.Verify(r => r.Update(userId, It.Is<User>(u => u.IsDeleted)), Times.Once);
        }

        [Test]
        public async Task UpdateUserStatusAsync_DisableBuyer_UpdatesUserOnly()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Role = UserRole.Buyer,
                IsDeleted = false
            };

            _userRepoMock.Setup(r => r.Get(userId)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.Update(userId, It.IsAny<User>())).ReturnsAsync(user);

            await _userService.UpdateUserStatusAsync(userId, true);

            Assert.IsTrue(user.IsDeleted);
            _listerProfileRepoMock.Verify(r => r.GetByUserIdAsync(It.IsAny<Guid>()), Times.Never);
            _userRepoMock.Verify(r => r.Update(userId, It.Is<User>(u => u.IsDeleted)), Times.Once);
        }

        [Test]
        public async Task UpdateUserStatusAsync_EnableLister_ResetsDeletion()
        {
            var userId = Guid.NewGuid();
            var profileId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Role = UserRole.Lister,
                IsDeleted = true
            };
            var profile = new ListerProfile
            {
                Id = profileId,
                UserId = userId,
                IsDeleted = true
            };

            _userRepoMock.Setup(r => r.Get(userId)).ReturnsAsync(user);
            _listerProfileRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(profile);
            _listerProfileRepoMock.Setup(r => r.Update(profileId, It.IsAny<ListerProfile>())).ReturnsAsync(profile);
            _userRepoMock.Setup(r => r.Update(userId, It.IsAny<User>())).ReturnsAsync(user);

            await _userService.UpdateUserStatusAsync(userId, false);

            Assert.IsFalse(user.IsDeleted);
            Assert.IsFalse(profile.IsDeleted);
            _listerProfileRepoMock.Verify(r => r.Update(profileId, It.Is<ListerProfile>(p => !p.IsDeleted)), Times.Once);
            _userRepoMock.Verify(r => r.Update(userId, It.Is<User>(u => !u.IsDeleted)), Times.Once);
        }

        [Test]
        public async Task UpdateUserStatusAsync_ListerProfileNotFound_OnlyUserUpdated()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Role = UserRole.Lister,
                IsDeleted = false
            };

            _userRepoMock.Setup(r => r.Get(userId)).ReturnsAsync(user);
            _listerProfileRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((ListerProfile)null);
            _userRepoMock.Setup(r => r.Update(userId, It.IsAny<User>())).ReturnsAsync(user);

            await _userService.UpdateUserStatusAsync(userId, true);

            Assert.IsTrue(user.IsDeleted);
            _listerProfileRepoMock.Verify(r => r.Update(It.IsAny<Guid>(), It.IsAny<ListerProfile>()), Times.Never);
            _userRepoMock.Verify(r => r.Update(userId, It.Is<User>(u => u.IsDeleted)), Times.Once);
        }
        
        [Test]
        public async Task UpdateUserStatusAsync_EnableBuyer_ResetsUserDeletionOnly()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Role = UserRole.Buyer,
                IsDeleted = true
            };

            _userRepoMock.Setup(r => r.Get(userId)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.Update(userId, It.IsAny<User>())).ReturnsAsync(user);

            await _userService.UpdateUserStatusAsync(userId, false);

            Assert.IsFalse(user.IsDeleted);
            _userRepoMock.Verify(r => r.Update(userId, It.Is<User>(u => !u.IsDeleted)), Times.Once);
            _listerProfileRepoMock.Verify(r => r.Update(It.IsAny<Guid>(), It.IsAny<ListerProfile>()), Times.Never);
        }
    }
}
