using Domin.System.Entities;
using Domin.System.IRepository.IUnitRepository;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.Test.IRepository
{
    public class IAllUnitRepositoryTests
    {
        private readonly Mock<IAllUnitRepository> _mockRepo;
        private readonly List<Unit> _testUnits;
        private readonly List<Branch> _testBranches;

        public IAllUnitRepositoryTests()
        {
            // Initialize Arabic test data
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
                Name = "فرع جدة",
                Address = "شارع الأمير محمد",
                City = "جدة",
                Phone = "0507654321",
                IsActive = true
            }
        };

            _testUnits = new List<Unit>
        {
            new Unit {
                Id_Unit = 1,
                Name = "كيلو",
                Branch_Id = 1,
                Branch = _testBranches[0]
            },
            new Unit {
                Id_Unit = 2,
                Name = "لتر",
                Branch_Id = 1,
                Branch = _testBranches[0]
            },
            new Unit {
                Id_Unit = 3,
                Name = "علبة",
                Branch_Id = 2,
                Branch = _testBranches[1]
            }
        };

            _mockRepo = new Mock<IAllUnitRepository>();

            /* Base Repository Methods */
            var mockDbSet = new Mock<DbSet<Unit>>();
            _mockRepo.Setup(r => r.Db).Returns(mockDbSet.Object);

            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _testUnits.FirstOrDefault(u => u.Id_Unit == id));

            _mockRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(_testUnits);

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Unit>()))
                .ReturnsAsync((Unit u) =>
                {
                    var validationResults = new List<ValidationResult>();
                    if (!Validator.TryValidateObject(u, new ValidationContext(u), validationResults, true))
                    {
                        throw new ValidationException(validationResults.First().ErrorMessage);
                    }
                    _testUnits.Add(u);
                    return u;
                });

            
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Unit>()))
                .ReturnsAsync((Unit u) =>
                {
                    var validationResults = new List<ValidationResult>();
                    if (!Validator.TryValidateObject(u, new ValidationContext(u), validationResults, true))
                    {
                        throw new ValidationException(validationResults.First().ErrorMessage);
                    }

                    var existing = _testUnits.FirstOrDefault(x => x.Id_Unit == u.Id_Unit);
                    if (existing != null)
                    {
                        existing.Name = u.Name;
                        existing.Branch_Id = u.Branch_Id;
                        // Update the Branch navigation property
                        existing.Branch = _testBranches.FirstOrDefault(b => b.Id_Branch == u.Branch_Id);
                    }
                    return existing;
                });

            _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var toDelete = _testUnits.FirstOrDefault(u => u.Id_Unit == id);
                    if (toDelete != null) _testUnits.Remove(toDelete);
                    return toDelete;
                });

            /* Custom Methods */
            _mockRepo.Setup(r => r.GetAllUnitsByBranch(It.IsAny<int>()))
                .ReturnsAsync((int branchId) => _testUnits
                    .Where(u => u.Branch_Id == branchId)
                    .ToList());

            _mockRepo.Setup(r => r.GetAllIncludeToBranchAsync())
                .ReturnsAsync(() => _testUnits
                    .Select(u => new Unit
                    {
                        Id_Unit = u.Id_Unit,
                        Name = u.Name,
                        Branch = u.Branch
                    })
                    .ToList());
        }

        /* Base Repository Tests */
        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsUnit()
        {
            var result = await _mockRepo.Object.GetByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal("كيلو", result.Name);
        }

        /* Custom Method Tests */
        [Fact]
        public async Task GetAllIncludeToBranchAsync_ReturnsUnitsWithBranches()
        {
            var result = await _mockRepo.Object.GetAllIncludeToBranchAsync();

            Assert.Equal(3, result.Count);
            Assert.All(result, u => Assert.NotNull(u.Branch));
            Assert.Equal("الفرع الرئيسي", result[0].Branch.Name);
        }

        [Theory]
        [InlineData(1, 2)] // Branch 1 has 2 units
        [InlineData(2, 1)] // Branch 2 has 1 unit
        [InlineData(3, 0)] // Non-existent branch
        public async Task GetAllUnitsByBranch_ReturnsCorrectCount(int branchId, int expectedCount)
        {
            var result = await _mockRepo.Object.GetAllUnitsByBranch(branchId);
            Assert.Equal(expectedCount, result.Count);
        }

        /* Validation Tests */
        [Theory]
        [InlineData("ك", 1, "يجب أن يكون اسم الوحدة بين 2 و100 حرف")] // Name too short
        [InlineData("كيلو", 0, "معرف الفرع غير صالح")] // Invalid branch
        public async Task AddAsync_InvalidData_ThrowsValidationException(
            string name, int branchId, string expectedError)
        {
            var invalidUnit = new Unit
            {
                Name = name,
                Branch_Id = branchId
            };

            var ex = await Assert.ThrowsAsync<ValidationException>(() =>
                _mockRepo.Object.AddAsync(invalidUnit));

            Assert.Contains(expectedError, ex.Message);
        }

        /* Relationship Tests */
        [Fact]
        public async Task GetAllIncludeToBranchAsync_IncludesBranchDetails()
        {
            var result = await _mockRepo.Object.GetAllIncludeToBranchAsync();
            Assert.Equal("الرياض", result[0].Branch.City);
        }

        [Fact]
        public async Task GetAllUnitsByBranch_FiltersCorrectly()
        {
            var result = await _mockRepo.Object.GetAllUnitsByBranch(1);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, u => u.Name == "كيلو");
            Assert.Contains(result, u => u.Name == "لتر");
            Assert.DoesNotContain(result, u => u.Name == "علبة");
        }

        /* Edge Cases */
        [Fact]
        public async Task GetAllUnitsByBranch_InvalidBranch_ReturnsEmpty()
        {
            var result = await _mockRepo.Object.GetAllUnitsByBranch(0);
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateAsync_ChangeBranch_UpdatesRelationship()
        {
            var update = new Unit
            {
                Id_Unit = 1,
                Name = "كيلو",
                Branch_Id = 2 // Change to branch 2
            };

            var result = await _mockRepo.Object.UpdateAsync(update);
            Assert.Equal(2, result.Branch_Id);
            Assert.Equal("فرع جدة", result.Branch.Name);
        }
    }
}
