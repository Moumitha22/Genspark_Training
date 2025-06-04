using ClinicManagementSystem.Contexts;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repositories;
using ClinicManagementSystem.Interfaces;

namespace ClinicManagementSystem.Test;
public class DoctorRepoTest
{
    ClinicContext _context;
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ClinicContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

        _context = new ClinicContext(options);
        
    }

    private User CreateTestUser(string email = "test@gmail.com", string role = "Doctor")
    {
        return new User
        {
            Username = email,
            Password = System.Text.Encoding.UTF8.GetBytes("test123"),
            HashKey = Guid.NewGuid().ToByteArray(),
            Role = role
        };
    }


    private Doctor CreateTestDoctor(string email = "test@gmail.com")
    {
        return new Doctor
        {
            Name = "Test Doctor",
            YearsOfExperience = 5,
            Email = email
        };
    }

    // [Test]
    // public async Task AddDoctorPassTest()
    // {
    //     //arrange
    //      var email = "test@gmail.com";
    //     _context.Users.Add(CreateTestUser(email));
    //     await _context.SaveChangesAsync();

    //     var doctor = CreateTestDoctor(email);

    //     IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);

    //     //action
    //     var result = await _doctorRepository.Add(doctor);

    //     //assert
    //     Assert.That(result, Is.Not.Null, "Doctor IS not added");
    //     Assert.That(result.Id, Is.EqualTo(1));
    //     Assert.That(result.Email, Is.EqualTo(email));
    // }

    // [TestCase(1)]
    // public async Task GetDoctorPassTest(int id)
    // {
    //     //arrange
    //     var email = "test@gmail.com";
    //     _context.Users.Add(CreateTestUser(email));
    //     await _context.SaveChangesAsync();

    //     var doctor = CreateTestDoctor(email);
    //     IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);
    //     await _doctorRepository.Add(doctor);

    //     //action
    //     var result = _doctorRepository.Get(id);

    //     //assert
    //     Assert.That(result.Id, Is.EqualTo(id));

    // }

    // [TestCase(3)]
    // public async Task GetDoctorExceptionTest(int id)
    // {
    //     //arrange
    //     var email = "test@gmail.com";
    //     _context.Users.Add(CreateTestUser(email));
    //     await _context.SaveChangesAsync();

    //     var doctor = CreateTestDoctor(email);
    //     IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);
    //     await _doctorRepository.Add(doctor);

    //     //action
    //     var result = _doctorRepository.Get(id);

    //     var ex = Assert.ThrowsAsync<Exception>(async () => await _doctorRepository.Get(id));
    //     Assert.That(ex.Message, Is.EqualTo($"No doctor with given ID {id}"));
    // }

    // [Test]
    // public async Task GetAllDoctorsPassTest()
    // {
    //     // Arrange
    //     var doctors = new List<Doctor>
    //     {
    //         CreateTestDoctor("doc1@gmail.com"),
    //         CreateTestDoctor("doc2@gmail.com"),
    //         CreateTestDoctor("doc3@gmail.com"),
    //     };

    //     IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);
    //     foreach (var doc in doctors)
    //     {
    //         await _doctorRepository.Add(doc);
    //     }
    //     // Action
    //     var result = await _doctorRepository.GetAll();
    //     // Assert
    //     var results = result.ToList();
    //     Assert.That(results.Count, Is.EqualTo(doctors.Count));
    //     CollectionAssert.AreEquivalent(
    //         doctors.ConvertAll(d => d.Email),
    //         results.ConvertAll(d => d.Email));
    // }

    // [Test]
    // public async Task GetAllDoctorsExceptionTest()
    // {
    //     // Arrange

    //     IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);

    //     // Action & Assert
    //     var ex = Assert.ThrowsAsync<Exception>(async () => await _doctorRepository.GetAll());
    //     Assert.That(ex.Message, Is.EqualTo($"No doctors in the database"));;
    // }

    [TestCase(1)]
    [TestCase(99)]
    public async Task UpdateDoctorTest(int id)
    {
        // Arrange
        var doctor = CreateTestDoctor();
        IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);
        doctor = await _doctorRepository.Add(doctor);

        doctor.Name = "Updated Name";
        doctor.YearsOfExperience = 10;

        // Action
        var updatedDoctor = await _doctorRepository.Update(id, doctor);

        //Assert
        Assert.That(updatedDoctor, Is.Not.Null);
        Assert.That(updatedDoctor.Id, Is.EqualTo(id));
        Assert.That(updatedDoctor.Name, Is.EqualTo("Updated Name"));
        Assert.That(updatedDoctor.YearsOfExperience, Is.EqualTo(10));
    }

    // [TestCase(999)]
    // public async Task UpdateDoctorExceptionTest(int id)
    // {
    //     // Arrange
    //     var doctor = CreateTestDoctor();
    //     IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);
    //     doctor = await _doctorRepository.Add(doctor);

    //     doctor.Id = id;
    //     doctor.Name = "Updated Name";

    //     // Action & Assert
    //     var ex = Assert.ThrowsAsync<System.Exception>(async () =>
    //         await _doctorRepository.Update(id, doctor));

    //     Assert.That(ex.Message, Is.EqualTo($"No doctor with given ID {id}"));
    // }


    // [Test]
    // public async Task DeleteDoctorTest()
    // {
    //     IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);
    //     var doctor = await _doctorRepository.Add(CreateTestDoctor());

    //     var deleteResult = await _doctorRepository.Delete(doctor.Id);

    //     Assert.That(deleteResult.Id, Is.EqualTo(doctor.Id));

    //     var ex = Assert.ThrowsAsync<System.Exception>(async () => await _doctorRepository.Get(doctor.Id));
    //     Assert.That(ex.Message, Is.EqualTo("No Doctor with the given ID"));
    // }

    // [Test]
    // public void DeleteDoctor_NotFound_ThrowsException()
    // {
    //     IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);

    //     var ex = Assert.ThrowsAsync<System.Exception>(async () =>
    //         await _doctorRepository.Delete(999)); 

    //     Assert.That(ex.Message, Is.EqualTo("No Doctor with the given ID"));
    // }


    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}