using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domin.System.IRepository.IBaseRepository.IAllBaseRepository;
using global::System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Moq;
namespace Domin.System.Test.IRepository
{
   

    namespace YourNamespace.Tests
    {
        public class IAllBaseRepositoryTests
        {
            private readonly Mock<IAllBaseRepository<TestEntity>> _mockRepo;
            private readonly List<TestEntity> _testData = new List<TestEntity>
        {
            new TestEntity { Id = 1, Name = "Entity 1" },
            new TestEntity { Id = 2, Name = "Entity 2" }
        };

            public IAllBaseRepositoryTests()
            {
                _mockRepo = new Mock<IAllBaseRepository<TestEntity>>();

                // Setup mock implementations
                _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync((int id) => _testData.FirstOrDefault(e => e.Id == id));

                _mockRepo.Setup(r => r.GetAllAsync())
                    .ReturnsAsync(_testData);

                _mockRepo.Setup(r => r.AddAsync(It.IsAny<TestEntity>()))
                    .ReturnsAsync((TestEntity e) => e);

                _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<TestEntity>()))
                    .ReturnsAsync((TestEntity e) => e);

                _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
                    .ReturnsAsync((int id) => _testData.First(e => e.Id == id));

                _mockRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<TestEntity, bool>>>()))
                    .ReturnsAsync((Expression<Func<TestEntity, bool>> predicate) =>
                        _testData.Any(predicate.Compile()));
            }

            public class TestEntity
            {
                public int Id { get; set; }
                public string Name { get; set; }
            }

            [Fact]
            public async Task GetByIdAsync_ExistingId_ReturnsEntity()
            {
                // Act
                var result = await _mockRepo.Object.GetByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
            }

            [Fact]
            public async Task GetByIdAsync_NonExistingId_ReturnsNull()
            {
                // Act
                var result = await _mockRepo.Object.GetByIdAsync(99);

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public async Task GetAllAsync_ReturnsAllEntities()
            {
                // Act
                var result = await _mockRepo.Object.GetAllAsync();

                // Assert
                Assert.Equal(2, result.Count);
            }

            [Fact]
            public async Task AddAsync_ValidEntity_ReturnsAddedEntity()
            {
                // Arrange
                var newEntity = new TestEntity { Id = 3, Name = "New Entity" };

                // Act
                var result = await _mockRepo.Object.AddAsync(newEntity);

                // Assert
                Assert.Equal(newEntity.Id, result.Id);
                _mockRepo.Verify(r => r.AddAsync(newEntity), Times.Once);
            }

            [Fact]
            public async Task UpdateAsync_ValidEntity_ReturnsUpdatedEntity()
            {
                // Arrange
                var entity = new TestEntity { Id = 1, Name = "Updated" };

                // Act
                var result = await _mockRepo.Object.UpdateAsync(entity);

                // Assert
                Assert.Equal("Updated", result.Name);
                _mockRepo.Verify(r => r.UpdateAsync(entity), Times.Once);
            }

            [Fact]
            public async Task DeleteAsync_ExistingId_ReturnsDeletedEntity()
            {
                // Act
                var result = await _mockRepo.Object.DeleteAsync(1);

                // Assert
                Assert.Equal(1, result.Id);
                _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
            }

            [Fact]
            public async Task AnyAsync_PredicateMatches_ReturnsTrue()
            {
                // Act
                var result = await _mockRepo.Object.AnyAsync(e => e.Id == 1);

                // Assert
                Assert.True(result);
            }

            [Fact]
            public async Task AnyAsync_PredicateDoesNotMatch_ReturnsFalse()
            {
                // Act
                var result = await _mockRepo.Object.AnyAsync(e => e.Id == 99);

                // Assert
                Assert.False(result);
            }
        }
    }

}
