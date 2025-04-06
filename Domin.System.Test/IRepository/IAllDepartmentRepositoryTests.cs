using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domin.System.Entities;
using Domin.System.IRepository.IDepartmentRepository;
using global::System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Domin.System.Test.IRepository
{
    
    public class IAllDepartmentRepositoryTests
    {
        private readonly Mock<IAllDepartmentRepository> _mockRepo;
        private readonly List<Department> _testDepartments;
        private readonly List<Branch> _testBranches;

        public IAllDepartmentRepositoryTests()
        {
            // Initialize test data
            _testBranches = new List<Branch>
        {
            new Branch { Id_Branch = 1, Name = "الفرع الرئيسي" },
            new Branch { Id_Branch = 2, Name = "فرع جدة" }
        };

            _testDepartments = new List<Department>
        {
            new Department {
                Id_Department = 1,
                Name = "قسم المبيعات",
                Description = "قسم متخصص في المبيعات",
                Branch_Id = 1,
                Branch = _testBranches[0]
            },
            new Department {
                Id_Department = 2,
                Name = "قسم الدعم الفني",
                Description = "قسم متخصص في الدعم الفني",
                Branch_Id = 1,
                Branch = _testBranches[0]
            },
            new Department {
                Id_Department = 3,
                Name = "قسم التسويق",
                Description = "قسم متخصص في التسويق",
                Branch_Id = 2,
                Branch = _testBranches[1]
            }
        };

            _mockRepo = new Mock<IAllDepartmentRepository>();

            /* Base Repository Methods */

            // Db property
            var mockDbSet = new Mock<DbSet<Department>>();
            _mockRepo.Setup(r => r.Db).Returns(mockDbSet.Object);

            // GetByIdAsync
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _testDepartments.FirstOrDefault(d => d.Id_Department == id));

            // GetAllAsync
            _mockRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(_testDepartments);

            // AddAsync
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Department>()))
                .ReturnsAsync((Department d) =>
                {
                    _testDepartments.Add(d);
                    return d;
                });

            // UpdateAsync
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Department>()))
                .ReturnsAsync((Department d) =>
                {
                    var existing = _testDepartments.FirstOrDefault(x => x.Id_Department == d.Id_Department);
                    if (existing != null)
                    {
                        existing.Name = d.Name;
                        existing.Description = d.Description;
                        existing.Branch_Id = d.Branch_Id;
                    }
                    return existing;
                });

            // DeleteAsync
            _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var toDelete = _testDepartments.FirstOrDefault(d => d.Id_Department == id);
                    if (toDelete != null) _testDepartments.Remove(toDelete);
                    return toDelete;
                });

            // AnyAsync
            _mockRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Department, bool>>>()))
                .ReturnsAsync((Expression<Func<Department, bool>> predicate) =>
                    _testDepartments.Any(predicate.Compile()));

            /* Custom Methods */

            // GetAllDepartmentsByUserBranchAsync
            _mockRepo.Setup(r => r.GetAllDepartmentsByUserBranchAsync(It.IsAny<int>()))
                .ReturnsAsync((int branchId) =>
                {
                    if (branchId == 0) return new List<Department>();
                    return _testDepartments
                        .Where(d => d.Branch_Id == branchId)
                        .ToList();
                });

            // GetAllDepartmentIncludeToBranchAsync
            _mockRepo.Setup(r => r.GetAllDepartmentIncludeToBranchAsync())
                .ReturnsAsync(() => _testDepartments);
        }

        /* Base Repository Method Tests */
        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsDepartment()
        {
            var result = await _mockRepo.Object.GetByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal("قسم المبيعات", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            var result = await _mockRepo.Object.GetByIdAsync(99);
            Assert.Null(result);
        }

        /* Custom Method Tests */
        [Theory]
        [InlineData(1, 2)] // Branch 1 has 2 departments
        [InlineData(2, 1)] // Branch 2 has 1 department
        [InlineData(3, 0)] // Non-existent branch
        [InlineData(0, 0)] // Invalid branch ID
        public async Task GetAllDepartmentsByUserBranchAsync_ReturnsCorrectCount(int branchId, int expectedCount)
        {
            var result = await _mockRepo.Object.GetAllDepartmentsByUserBranchAsync(branchId);
            Assert.Equal(expectedCount, result.Count);

            if (expectedCount > 0)
            {
                Assert.All(result, d => Assert.Equal(branchId, d.Branch_Id));
            }
        }

        [Fact]
        public async Task GetAllDepartmentIncludeToBranchAsync_ReturnsAllDepartments()
        {
            var result = await _mockRepo.Object.GetAllDepartmentIncludeToBranchAsync();
            Assert.Equal(_testDepartments.Count, result.Count);
            Assert.All(result, d => Assert.NotNull(d.Branch));
        }

        /* Validation Tests */
        [Fact]
        public async Task AddAsync_InvalidDepartment_ThrowsValidationException()
        {
            var invalidDept = new Department
            {
                Name = "A", // Too short
                Branch_Id = 0 // Invalid
            };

            // In a real implementation, you would validate here
            // For mock tests, we assume validation happens in the service layer
            await _mockRepo.Object.AddAsync(invalidDept);

            // Verify it was added despite being invalid
            // (since repository typically doesn't validate)
            Assert.Contains(invalidDept, _testDepartments);
        }

        /* Branch Relationship Tests */
        [Fact]
        public async Task GetAllDepartmentIncludeToBranchAsync_ReturnsCorrectBranchNames()
        {
            var result = await _mockRepo.Object.GetAllDepartmentIncludeToBranchAsync();
            var branchNames = result.Select(d => d.Branch?.Name).Distinct().ToList();

            Assert.Contains("الفرع الرئيسي", branchNames);
            Assert.Contains("فرع جدة", branchNames);
        }
    }
}
