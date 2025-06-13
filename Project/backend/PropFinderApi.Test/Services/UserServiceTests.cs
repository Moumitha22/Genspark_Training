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
        private Mock<IAgentProfileRepository> _agentProfileRepoMock;
        private IUserService _userService;

        [SetUp]
        public void SetUp()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _agentProfileRepoMock = new Mock<IAgentProfileRepository>();
            _userService = new UserService(_userRepoMock.Object, _agentProfileRepoMock.Object);
        }

        [Test]
        public async Task CreateUserAsync_ValidUser_ReturnsCreatedUser()
        {
            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };
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
             var user = new User { Id =  id, Email = "test@example.com" };
            _userRepoMock.Setup(r => r.Get(id)).ReturnsAsync(user);

            var result = await _userService.GetUserByIdAsync(id);

            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(id));
        }

        [Test]
        public async Task GetUserByIdAsync_UserNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userRepoMock.Setup(r => r.Get(userId))
                        .ThrowsAsync(new NotFoundException("User not found"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
                await _userService.GetUserByIdAsync(userId));

            Assert.That(ex.Message, Is.EqualTo("User not found"));
        }

        [Test]
        public async Task GetUserByEmailAsync_TrimmedLoweredEmail_ReturnsUser()
        {
            var email = "USER@Example.com";
            var cleanedEmail = "user@example.com";
            var user = new User { Email = cleanedEmail };

            _userRepoMock.Setup(r => r.GetByEmailAsync(cleanedEmail)).ReturnsAsync(user);

            var result = await _userService.GetUserByEmailAsync("  USER@Example.com  ");

            Assert.IsNotNull(result);
            Assert.That(result.Email, Is.EqualTo(cleanedEmail));
        }

        [Test]
        public async Task GetAllUsersAsync_ReturnsUserList()
        {
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = "user1@example.com" },
                new User { Id = Guid.NewGuid(), Email = "user2@example.com" }
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
        public void UpdateUserAsync_NonAdminWrongUser_ThrowsUnauthorized()
        {
            var requesterUserId = Guid.NewGuid();    
            var targetUpdateUserId = Guid.NewGuid();

            var dto = new UserUpdateRequestDto();

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedException>(async () =>
                await _userService.UpdateUserAsync(targetUpdateUserId, dto, requesterUserId, "Buyer"));

            Assert.That(ex.Message, Is.EqualTo("You are not authorized to update this data."));
        }

        [Test]
        public async Task DeleteUserAsync_AgentAlsoDeletesProfile()
        {
            var id = Guid.NewGuid();
            var agent = new User { Id = id, Role = UserRole.Agent };
            var profile = new AgentProfile { Id = Guid.NewGuid(), UserId = id };

            _userRepoMock.Setup(r => r.Get(id)).ReturnsAsync(agent);
            _agentProfileRepoMock.Setup(r => r.GetByUserIdAsync(id)).ReturnsAsync(profile);
            _agentProfileRepoMock.Setup(r => r.Update(profile.Id, It.IsAny<AgentProfile>()))
                .ReturnsAsync((Guid _, AgentProfile p) => p);
            _userRepoMock.Setup(r => r.Update(id, It.IsAny<User>())).ReturnsAsync(agent);

            await _userService.DeleteUserAsync(id);

            _userRepoMock.Verify(r => r.Update(id, It.Is<User>(u => u.IsDeleted)), Times.Once);
            _agentProfileRepoMock.Verify(p => p.Update(profile.Id, It.Is<AgentProfile>(ap => ap.IsDeleted)), Times.Once);
        }

        [Test]
        public async Task DeleteUserAsync_Buyer_NoProfileDeleted()
        {
            var id = Guid.NewGuid();
            var buyer = new User { Id = id, Role = UserRole.Buyer };

            _userRepoMock.Setup(r => r.Get(id)).ReturnsAsync(buyer);
            _userRepoMock.Setup(r => r.Update(id, It.IsAny<User>())).ReturnsAsync(buyer);

            await _userService.DeleteUserAsync(id);

            _agentProfileRepoMock.Verify(p => p.Update(It.IsAny<Guid>(), It.IsAny<AgentProfile>()), Times.Never);
            _userRepoMock.Verify(r => r.Update(id, It.Is<User>(u => u.IsDeleted)), Times.Once);
        }

        [Test]
        public void DeleteUserAsync_UserNotFound_ThrowsNotFoundException()
        {
            var nonExistentUserId = Guid.NewGuid();
            _userRepoMock.Setup(r => r.Get(nonExistentUserId)).ThrowsAsync(new NotFoundException("User not found"));

            var ex = Assert.ThrowsAsync<NotFoundException>(() => _userService.DeleteUserAsync(nonExistentUserId));

            Assert.That(ex.Message, Is.EqualTo("User not found"));
            _userRepoMock.Verify(r => r.Update(It.IsAny<Guid>(), It.IsAny<User>()), Times.Never);
            _agentProfileRepoMock.Verify(p => p.Update(It.IsAny<Guid>(), It.IsAny<AgentProfile>()), Times.Never);

        }
    }
}
