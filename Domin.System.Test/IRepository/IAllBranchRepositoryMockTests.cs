using Domin.System.Entities;
using Domin.System.IRepository.IBranchRepository;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace Domin.System.Test.IRepository
{
    public class IAllBranchRepositoryTests
    {
        private readonly Mock<IAllBranchRepository> _mockRepo;
        private readonly List<Branch> _testBranches;

        public IAllBranchRepositoryTests()
        {
            // Initialize test data with complete Branch objects
            _testBranches = new List<Branch>
            {
                new Branch {
                    Id_Branch = 1,
                    Name = "الفرع الرئيسي",
                    Address = "شارع الملك عبدالعزيز",
                    City = "الرياض",
                    Phone = "0501234567",
                    IsActive = true
                },
                new Branch {
                    Id_Branch = 2,
                    Name = "الفرع الفرعي",
                    Address = "شارع الأمير محمد",
                    City = "جدة",
                    Phone = "0507654321",
                    IsActive = true
                }
            };

            _mockRepo = new Mock<IAllBranchRepository>();

            /* Base Repository Methods */

            // Db property
            var mockDbSet = new Mock<DbSet<Branch>>();
            _mockRepo.Setup(r => r.Db).Returns(mockDbSet.Object);

            // GetByIdAsync
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _testBranches.FirstOrDefault(b => b.Id_Branch == id));

            // GetAllAsync
            _mockRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(_testBranches);

            // AddAsync
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Branch>()))
                .ReturnsAsync((Branch b) =>
                {
                    _testBranches.Add(b);
                    return b;
                });

            // UpdateAsync
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Branch>()))
                .ReturnsAsync((Branch b) =>
                {
                    var existing = _testBranches.FirstOrDefault(x => x.Id_Branch == b.Id_Branch);
                    if (existing != null)
                    {
                        existing.Name = b.Name;
                        existing.Address = b.Address;
                        existing.City = b.City;
                        existing.Phone = b.Phone;
                        existing.IsActive = b.IsActive;
                    }
                    return existing;
                });

            // DeleteAsync
            _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var toDelete = _testBranches.FirstOrDefault(b => b.Id_Branch == id);
                    if (toDelete != null) _testBranches.Remove(toDelete);
                    return toDelete;
                });

            // AnyAsync
            _mockRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Branch, bool>>>()))
                .ReturnsAsync((Expression<Func<Branch, bool>> predicate) =>
                    _testBranches.Any(predicate.Compile()));

            /* Custom Method */

            // GetBranchNameById
            _mockRepo.Setup(r => r.GetBranchNameById(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                    _testBranches.Where(b => b.Id_Branch == id)
                                .Select(b => b.Name)
                                .FirstOrDefault());
        }

        
        
        
        
        // Custom Method Tests
        [Fact]
        public async Task GetBranchNameById_ExistingId_ReturnsCorrectName()
        {
            var result = await _mockRepo.Object.GetBranchNameById(1);
            Assert.Equal("الفرع الرئيسي", result);
        }

        [Fact]
        public async Task GetBranchNameById_NonExistingId_ReturnsNull()
        {
            var result = await _mockRepo.Object.GetBranchNameById(99);
            Assert.Null(result);
        }

        // Base Repository Method Tests
        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsBranch()
        {
            var result = await _mockRepo.Object.GetByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal(1, result.Id_Branch);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            var result = await _mockRepo.Object.GetByIdAsync(99);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllBranches()
        {
            var result = await _mockRepo.Object.GetAllAsync();
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task AddAsync_AddsNewBranch()
        {
            var newBranch = new Branch { Id_Branch = 3, Name = "فرع جديد" };
            var result = await _mockRepo.Object.AddAsync(newBranch);
            Assert.Equal(3, result.Id_Branch);
            Assert.Contains(newBranch, _testBranches);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesExistingBranch()
        {
            var updatedBranch = new Branch { Id_Branch = 1, Name = "الفرع الرئيسي المعدل" };
            var result = await _mockRepo.Object.UpdateAsync(updatedBranch);
            Assert.Equal("الفرع الرئيسي المعدل", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_NonExistingBranch_ReturnsNull()
        {
            var nonExistingBranch = new Branch { Id_Branch = 99 };
            var result = await _mockRepo.Object.UpdateAsync(nonExistingBranch);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_RemovesBranch()
        {
            var initialCount = _testBranches.Count;
            var result = await _mockRepo.Object.DeleteAsync(1);
            Assert.Equal(1, result.Id_Branch);
            Assert.Equal(initialCount - 1, _testBranches.Count);
        }

        [Fact]
        public async Task DeleteAsync_NonExistingId_ReturnsNull()
        {
            var result = await _mockRepo.Object.DeleteAsync(99);
            Assert.Null(result);
        }

        [Fact]
        public async Task AnyAsync_WithExistingBranch_ReturnsTrue()
        {
            var result = await _mockRepo.Object.AnyAsync(b => b.Id_Branch == 1);
            Assert.True(result);
        }

        [Fact]
        public async Task AnyAsync_WithNonExistingBranch_ReturnsFalse()
        {
            var result = await _mockRepo.Object.AnyAsync(b => b.Id_Branch == 99);
            Assert.False(result);
        }

        [Fact]
        public void Db_Property_ReturnsDbSet()
        {
            var result = _mockRepo.Object.Db;
            Assert.NotNull(result);
            Assert.IsAssignableFrom<DbSet<Branch>>(result);
        }
    }
}
