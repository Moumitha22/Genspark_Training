using ClinicManagementSystem.Contexts;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repositories;
using ClinicManagementSystem.Interfaces;

namespace ClinicManagementSystem.Test;
public class PatientRepoTest
{
    private ClinicContext _context;
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ClinicContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

        _context = new ClinicContext(options);
        
    }

    private User CreateTestUser(string email = "test@gmail.com", string role = "Patient")
    {
        return new User
        {
            Username = email,
            Password = System.Text.Encoding.UTF8.GetBytes("test123"),
            HashKey = Guid.NewGuid().ToByteArray(),
            Role = role
        };
    }


    private Patient CreateTestPatient(string email = "test@gmail.com")
    {
        return new Patient
        {
            Name = "Test Patient",
            Email = email
        };
    }

    // [Test]
    // public async Task AddPatientPassTest()
    // {
    //     // Arrange
    //      var email = "test@gmail.com";
    //     _context.Users.Add(CreateTestUser(email));
    //     await _context.SaveChangesAsync();

    //     var patient = CreateTestPatient(email);

    //     IRepository<int, Patient> _patientRepository = new PatientRepository(_context);

    //     // Action
    //     var result = await _patientRepository.Add(patient);

    //     // Assert
    //     Assert.That(result, Is.Not.Null, "Patient IS not added");
    //     Assert.That(result.Id, Is.EqualTo(1));
    //     Assert.That(result.Email, Is.EqualTo(email));
    // }

    // [TestCase(1)]
    // public async Task GetPatientPassTest(int id)
    // {
    //     //arrange
    //     var email = "test@gmail.com";
    //     _context.Users.Add(CreateTestUser(email));
    //     await _context.SaveChangesAsync();

    //     var patient = CreateTestPatient(email);
    //     IRepository<int, Patient> _patientRepository = new PatientRepository(_context);
    //     await _patientRepository.Add(patient);

    //     //action
    //     var result = _patientRepository.Get(id);

    //     //assert
    //     Assert.That(result.Id, Is.EqualTo(id));
    // }

    // [TestCase(3)]
    // public async Task GetPatientExceptionTest(int id)
    // {
    //     //arrange
    //     var email = "test@gmail.com";
    //     _context.Users.Add(CreateTestUser(email));
    //     await _context.SaveChangesAsync();

    //     var patient = CreateTestPatient(email);
    //     IRepository<int, Patient> _patientRepository = new PatientRepository(_context);
    //     await _patientRepository.Add(patient);

    //     //action
    //     var result = _patientRepository.Get(id);

    //     var ex = Assert.ThrowsAsync<Exception>(async () => await _patientRepository.Get(id));
    //     Assert.That(ex.Message, Is.EqualTo($"No patient with the given ID {id}"));
    // }

    // [Test]
    // public async Task GetAllPatientsPassTest()
    // {
    //     // Arrange
    //     var patients = new List<Patient>
    //     {
    //         CreateTestPatient("patient1@gmail.com"),
    //         CreateTestPatient("patient2@gmail.com"),
    //         CreateTestPatient("patient3@gmail.com"),
    //     };

    //     IRepository<int, Patient> _patientRepository = new PatientRepository(_context);
    //     foreach (var patient in patients)
    //     {
    //         await _patientRepository.Add(patient);
    //     }
    //     // Action
    //     var result = await _patientRepository.GetAll();
    //     // Assert
    //     var results = result.ToList();
    //     Assert.That(results.Count, Is.EqualTo(patients.Count));
    //     CollectionAssert.AreEquivalent(
    //         patients.ConvertAll(d => d.Email),
    //         results.ConvertAll(d => d.Email));
    // }

    // [Test]
    // public async Task GetAllPatientsExceptionTest()
    // {
    //     // Arrange

    //     IRepository<int, Patient> _patientRepository = new PatientRepository(_context);

    //     // Action & Assert
    //     var ex = Assert.ThrowsAsync<Exception>(async () => await _patientRepository.GetAll());
    //     Assert.That(ex.Message, Is.EqualTo($"No Patients in the database"));;
    // }

    // [TestCase(1)]
    // public async Task UpdatePatientTest(int id)
    // {
    //     // Arrange
    //     var patient = CreateTestPatient();
    //     IRepository<int, Patient> _patientRepository = new PatientRepository(_context);
    //     patient = await _patientRepository.Add(patient);

    //     patient.Name = "Updated Name";
    //     patient.Age = 30;

    //     // Action
    //     var updatedPatient = await _patientRepository.Update(id, patient);

    //     //Assert
    //     Assert.That(updatedPatient, Is.Not.Null);
    //     Assert.That(updatedPatient.Id, Is.EqualTo(id));
    //     Assert.That(updatedPatient.Name, Is.EqualTo("Updated Name"));
    //     Assert.That(updatedPatient.Age, Is.EqualTo(30));
    // }

    // [TestCase(999)]
    // public async Task UpdatePatientExceptionTest(int id)
    // {
    //     // Arrange
    //     var patient = CreateTestPatient();
    //     IRepository<int, Patient> _patientRepository = new PatientRepository(_context);
    //     patient = await _patientRepository.Add(patient);

    //     patient.Id = id;
    //     patient.Name = "Updated Name";

    //     // Action & Assert
    //     var ex = Assert.ThrowsAsync<System.Exception>(async () =>
    //         await _patientRepository.Update(id, patient));

    //     Assert.That(ex.Message, Is.EqualTo($"No patient with the given ID {id}"));
    // }


    // [TestCase(1)]
    // public async Task DeletePatientPassTest(int id)
    // {
    //      // Arrange
    //     IRepository<int, Patient> _patientRepository = new PatientRepository(_context);
    //     var patient = await _patientRepository.Add(CreateTestPatient());

    //     // Action 
    //     var deleteResult = await _patientRepository.Delete(id);

    //     // Assert
    //     Assert.That(deleteResult.Id, Is.EqualTo(id));
    // }

    // [TestCase(99)]
    // public void DeletePatientExceptionTest(int id)
    // {
    //     IRepository<int, Patient> _patientRepository = new PatientRepository(_context);

    //     // Action & Assert
    //     var ex = Assert.ThrowsAsync<System.Exception>(async () =>
    //         await _patientRepository.Delete(id)); 

    //     Assert.That(ex.Message, Is.EqualTo($"No patient with the given ID {id}"));
    // }


    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}