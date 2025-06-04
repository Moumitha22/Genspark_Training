using   ClinicManagementSystem.Contexts;
using   ClinicManagementSystem.Models;
using   ClinicManagementSystem.Repositories;
using   ClinicManagementSystem.Interfaces;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace   ClinicManagementSystem.Test
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

        [Test]
        public async Task AddSpecialityTest()
        {
            IRepository<int, Speciality> repository = new SpecialityRepository(_context);
            var speciality = CreateTestSpeciality();

            var result = await repository.Add(speciality);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.Name, Is.EqualTo("Cardiology"));
        }

        [Test]
        public async Task GetSpecialityTest()
        {
            IRepository<int, Speciality> repository = new SpecialityRepository(_context);
            var speciality = await repository.Add(CreateTestSpeciality("Neurology"));

            var result = await repository.Get(speciality.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Neurology"));
        }

        [Test]
        public void GetSpeciality_NotFound_ThrowsException()
        {
            IRepository<int, Speciality> repository = new SpecialityRepository(_context);

            var ex = Assert.ThrowsAsync<Exception>(async () => await repository.Get(999));
            Assert.That(ex.Message, Is.EqualTo("No speciality with the given ID"));
        }

        [Test]
        public async Task GetAllSpecialitiesTest()
        {
            IRepository<int, Speciality> repository = new SpecialityRepository(_context);
            var list = new List<Speciality>
            {
                CreateTestSpeciality("Cardiology"),
                CreateTestSpeciality("Orthopedics"),
                CreateTestSpeciality("Dermatology")
            };

            foreach (var item in list)
                await repository.Add(item);

            var result = await repository.GetAll();
            var results = result.ToList();

            Assert.That(results.Count, Is.EqualTo(list.Count));
            Assert.That(results.Select(s => s.Name), Is.EquivalentTo(list.Select(s => s.Name)));
        }

        [Test]
        public async Task UpdateSpecialityTest()
        {
            IRepository<int, Speciality> repository = new SpecialityRepository(_context);
            var speciality = await repository.Add(CreateTestSpeciality("ENT"));

            speciality.Status = "Inactive";
            var updated = await repository.Update(speciality.Id, speciality);

            Assert.That(updated.Status, Is.EqualTo("Inactive"));
        }

        [Test]
        public void UpdateSpeciality_NotFound_ThrowsException()
        {
            IRepository<int, Speciality> repository = new SpecialityRepository(_context);
            var speciality = CreateTestSpeciality();
            speciality.Id = 999;

            var ex = Assert.ThrowsAsync<Exception>(async () => await repository.Update(speciality.Id, speciality));
            Assert.That(ex.Message, Is.EqualTo("No speciality with the given ID"));
        }

        [Test]
        public async Task DeleteSpecialityTest()
        {
            IRepository<int, Speciality> repository = new SpecialityRepository(_context);
            var speciality = await repository.Add(CreateTestSpeciality());

            var deleted = await repository.Delete(speciality.Id);
            Assert.That(deleted.Id, Is.EqualTo(speciality.Id));

            var ex = Assert.ThrowsAsync<Exception>(async () => await repository.Get(speciality.Id));
            Assert.That(ex.Message, Is.EqualTo("No speciality with the given ID"));
        }

        [Test]
        public void DeleteSpeciality_NotFound_ThrowsException()
        {
            IRepository<int, Speciality> repository = new SpecialityRepository(_context);

            var ex = Assert.ThrowsAsync<Exception>(async () => await repository.Delete(999));
            Assert.That(ex.Message, Is.EqualTo("No speciality with the given ID"));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
