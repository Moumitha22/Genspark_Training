using NUnit.Framework;
using PropFinderApi.Services;
using PropFinderApi.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace PropFinderApi.Tests.Services
{
    public class PaginationServiceTests
    {
        private PaginationService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new PaginationService();
        }

        [Test]
        public void ApplyPagination_ShouldReturnCorrectPageData()
        {
            var data = Enumerable.Range(1, 20).ToList(); // 1 to 20
            int page = 2;
            int pageSize = 5;

            var (result, pagination) = _service.ApplyPagination(data, page, pageSize);

            Assert.AreEqual(5, result.Count);
            CollectionAssert.AreEqual(new List<int> { 6, 7, 8, 9, 10 }, result);
            Assert.AreEqual(4, pagination.TotalPages);
            Assert.AreEqual(20, pagination.TotalItems);
            Assert.AreEqual(5, pagination.PageSize);
            Assert.AreEqual(2, pagination.CurrentPage);
        }

        [Test]
        public void ApplyPagination_ShouldHandlePageBeyondLimit()
        {
            var data = Enumerable.Range(1, 8).ToList();
            int page = 5; // beyond total pages
            int pageSize = 3;

            var (result, pagination) = _service.ApplyPagination(data, page, pageSize);

            Assert.AreEqual(2, result.Count);
            CollectionAssert.AreEqual(new List<int> { 7, 8 }, result);
            Assert.AreEqual(3, pagination.TotalPages);
            Assert.AreEqual(8, pagination.TotalItems);
            Assert.AreEqual(3, pagination.PageSize);
            Assert.AreEqual(3, pagination.CurrentPage); // capped
        }

        [Test]
        public void ApplyPagination_ShouldReturnEmptyForEmptySource()
        {
            var data = new List<int>();
            int page = 1;
            int pageSize = 5;

            var (result, pagination) = _service.ApplyPagination(data, page, pageSize);

            Assert.IsEmpty(result);
            Assert.AreEqual(0, pagination.TotalItems);
            Assert.AreEqual(0, pagination.TotalPages);
            Assert.AreEqual(0, pagination.CurrentPage);
        }

        [Test]
        public void ApplyPagination_FirstPage_ShouldReturnCorrectData()
        {
            var data = Enumerable.Range(10, 10).ToList(); // 10 to 19
            int page = 1;
            int pageSize = 4;

            var (result, pagination) = _service.ApplyPagination(data, page, pageSize);

            Assert.AreEqual(4, result.Count);
            CollectionAssert.AreEqual(new List<int> { 10, 11, 12, 13 }, result);
            Assert.AreEqual(3, pagination.TotalPages);
            Assert.AreEqual(1, pagination.CurrentPage);
        }
    }
}
