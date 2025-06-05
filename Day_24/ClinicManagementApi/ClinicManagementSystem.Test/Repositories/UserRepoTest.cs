using ClinicManagementSystem.Contexts;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Interfaces;
using ClinicManagementSystem.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Test
{
    public class UserRepoTest
    {
        private ClinicContext _context;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ClinicContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new ClinicContext(options);
        }

        private User CreateSampleUser(string username = "testuser@example.com")
        {
            return new User
            {
                Username = username,
                Password = System.Text.Encoding.UTF8.GetBytes("test123"),
                HashKey = Guid.NewGuid().ToByteArray(),
                Role = "Patient"
            };
        }

        // [Test]
        // public async Task CreateUserPassTest()
        // {
        //     IRepository<string, User> userRepository = new UserRepository(_context);
        //     var user = CreateSampleUser();

        //     var savedUser = await userRepository.Add(user);

        //     Assert.That(savedUser, Is.Not.Null);
        //     Assert.That(savedUser.Username, Is.EqualTo(user.Username));
        // }

        // [Test]
        // public async Task GetUserPassTest()
        // {
        //     IRepository<string, User> userRepository = new UserRepository(_context);
        //     var user = CreateSampleUser();
        //     await userRepository.Add(user);

        //     var retrieved = await userRepository.Get(user.Username);

        //     Assert.That(retrieved.Username, Is.EqualTo(user.Username));
        // }

        // [TestCase("user1@gmail.com")]
        // public void GetUser_InvalidKey_ThrowsExceptionTest(string email)
        // {
        //     IRepository<string, User> userRepository = new UserRepository(_context);

        //     var ex = Assert.ThrowsAsync<Exception>(async () => await userRepository.Get(email));
        //     Assert.That(ex.Message, Is.EqualTo($"No user with given user name {email}"));
        // }

        // [Test]
        // public async Task GetAllUsersPassTest()
        // {
        //     IRepository<string, User> userRepository = new UserRepository(_context);
        //     var users = new[]
        //     {
        //         CreateSampleUser("user1@example.com"),
        //         CreateSampleUser("user2@example.com"),
        //         CreateSampleUser("user3@example.com")
        //     };

        //     foreach (var user in users)
        //         await userRepository.Add(user);

        //     var allUsers = (await userRepository.GetAll()).ToList();

        //     Assert.That(allUsers.Count, Is.EqualTo(users.Length));
        // }
        
        // [Test]
        // public async Task GetAllUsersExceptionTest()
        // {
        //     IRepository<string, User> userRepository = new UserRepository(_context);

        //     var ex = Assert.ThrowsAsync<Exception>(async () => await userRepository.GetAll());
        //     Assert.That(ex.Message, Is.EqualTo($"No users in the database"));;
        // }

        // [Test]
        // public async Task UpdateUser_Success()
        // {
        //     IRepository<string, User> userRepository = new UserRepository(_context);
        //     var user = await userRepository.Add(CreateSampleUser());

        //     user.Role = "Admin";
        //     var updated = await userRepository.Update(user.Username, user);

        //     Assert.That(updated.Role, Is.EqualTo("Admin"));
        // }


        //    [TestCase("test@gmail.com")]
        //     public void UpdateUser_NotFound_ThrowsException(string email)
        //     {
        //         IRepository<string, User> userRepository = new UserRepository(_context);
        //         var user = CreateSampleUser("user@gmail.com");

        //         var ex = Assert.ThrowsAsync<Exception>(async () => await userRepository.Update(email, user));
        //         Assert.That(ex.Message, Is.EqualTo($"No user with given user name {email}"));
        //     }

        // [Test]
        // public async Task DeleteUserPassTest()
        // {
        //     IRepository<string, User> userRepository = new UserRepository(_context);
        //     var user = await userRepository.Add(CreateSampleUser());

        //     var deleted = await userRepository.Delete(user.Username);

        //     Assert.That(deleted.Username, Is.EqualTo(user.Username));
        // }

        // [TestCase("usertest@gmail.com")]
        // public void DeleteUser_NotFound_ThrowsExceptionTest(string email)
        // {
        //     IRepository<string, User> userRepository = new UserRepository(_context);

        //     var ex = Assert.ThrowsAsync<Exception>(async () => await userRepository.Delete(email));
        //     Assert.That(ex.Message, Is.EqualTo($"No user with given user name {email}"));
        // }

        [TearDown]
        public void Cleanup()
        {
            _context.Dispose();
        }
    }
}