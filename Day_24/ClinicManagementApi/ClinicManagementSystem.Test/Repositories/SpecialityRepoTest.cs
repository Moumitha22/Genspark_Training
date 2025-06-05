using ClinicManagementSystem.Contexts;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repositories;
using ClinicManagementSystem.Interfaces;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Test
{
    public class SpecialityRepoTest
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

        private Speciality CreateTestSpeciality(string name = "Cardiology")
        {
            return new Speciality
            {
                Name = name,
                Status = "Active"
            };
        }

        // [Test]
        // public async Task AddSpecialityTest()
        // {
        //     // Arrange
        //     IRepository<int, Speciality> specialityRepository = new SpecialityRepository(_context);
        //     var speciality = CreateTestSpeciality();

        //     // Action
        //     var result = await specialityRepository.Add(speciality);

        //     // Assert
        //     Assert.That(result, Is.Not.Null);
        //     Assert.That(result.Id, Is.GreaterThan(0));
        //     Assert.That(result.Name, Is.EqualTo("Cardiology"));
        // }

        // [TestCase(1)]
        // public async Task GetSpecialityPassTest(int id)
        // {
            // Arrange
        //     IRepository<int, Speciality> specialityRepository = new SpecialityRepository(_context);
        //     var speciality = await specialityRepository.Add(CreateTestSpeciality("Neurology"));

        //     // Action
        //     var result = await specialityRepository.Get(id);

        //     // Assert
        //     Assert.That(result, Is.Not.Null);
        //     Assert.That(result.Name, Is.EqualTo("Neurology"));
        // }

        // [TestCase(99)]
        // public void GetSpecialityExceptionTest(int id)
        // {
        //     // Arrange
        //     IRepository<int, Speciality> specialityRepository = new SpecialityRepository(_context);

        //     // Action & Assert
        //     var ex = Assert.ThrowsAsync<Exception>(async () => await specialityRepository.Get(id));
        //     Assert.That(ex.Message, Is.EqualTo($"No speciality with given ID {id}"));
        // }

        // [Test]
        // public async Task GetAllSpecialitiesPassTest()
        // {
        //     // Arrange
        //     IRepository<int, Speciality> specialityRepository = new SpecialityRepository(_context);
        //     var specialities = new List<Speciality>
        //     {
        //         CreateTestSpeciality("Cardiology"),
        //         CreateTestSpeciality("Neurology"),
        //         CreateTestSpeciality("Dermatology")
        //     };

        //     foreach (var item in specialities)
        //         await specialityRepository.Add(item);

        //     // Action
        //     var result = await specialityRepository.GetAll();
        //     var results = result.ToList();

        //     // Assert
        //     Assert.That(results.Count, Is.EqualTo(specialities.Count));
        //     Assert.That(results.Select(s => s.Name), Is.EquivalentTo(specialities.Select(s => s.Name)));
        // }
        
        // [Test]
        // public async Task GetAllSpecialitiesExceptionTest()
        // {
        //     // Arrange
        //     IRepository<int, Speciality> specialityRepository = new SpecialityRepository(_context);

        //     // Action & Assert
        //     var ex = Assert.ThrowsAsync<Exception>(async () => await specialityRepository.GetAll());
        //     Assert.That(ex.Message, Is.EqualTo($"No specialities in the database"));;
        // }

        // [TestCase]
        // public async Task UpdateSpecialityPassTest()
        // {
        //     // Arrange
        //     IRepository<int, Speciality> specialityRepository = new SpecialityRepository(_context);
        //     var speciality = await specialityRepository.Add(CreateTestSpeciality("ENT"));

        //     // Action
        //     speciality.Status = "Inactive";
        //     var updatedSpeciality = await specialityRepository.Update(speciality.Id, speciality);

        //     // Assert
        //     Assert.That(updatedSpeciality.Status, Is.EqualTo("Inactive"));
        // }

        // [Test]
        // public void UpdateSpecialityThrowsExceptionTest()
        // {
        //     // Arrange
        //     IRepository<int, Speciality> specialityRepository = new SpecialityRepository(_context);
        //     var speciality = CreateTestSpeciality();

        //     speciality.Id = 999;
        //     speciality.Status = "Inactive";

        //     // Action & Assert
        //     var ex = Assert.ThrowsAsync<Exception>(async () => await specialityRepository.Update(speciality.Id, speciality));
        //     Assert.That(ex.Message, Is.EqualTo($"No speciality with given ID {speciality.Id}"));
        // }

        // [Test]
        // public async Task DeleteSpecialityPassTest()
        // {
        //     // Arrange
        //     IRepository<int, Speciality> specialityRepository = new SpecialityRepository(_context);
        //     var speciality = await specialityRepository.Add(CreateTestSpeciality());

        //     // Action 
        //     var deleted = await specialityRepository.Delete(speciality.Id);

        //     // Assert
        //     Assert.That(deleted.Id, Is.EqualTo(speciality.Id));
        // }

        // [TestCase(99)]
        // public void DeleteSpecialityThrowsExceptionTest(int id)
        // {
        //     // Arrange
        //     IRepository<int, Speciality> specialityRepository = new SpecialityRepository(_context);

        //     // Action & Assert
        //     var ex = Assert.ThrowsAsync<Exception>(async () => await specialityRepository.Delete(id));
        //     Assert.That(ex.Message, Is.EqualTo($"No speciality with given ID {id}"));
        // }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
